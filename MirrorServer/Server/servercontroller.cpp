#include "servercontroller.hpp"
#include "serversocket.hpp"

#include "qhttpserver.h"
#include "qhttprequest.h"
#include "qhttpresponse.h"

#include <QBuffer>
#include <QTcpSocket>


namespace mirrors {

ServerController::ServerController(QObject *parent)
    : QObject(parent),
      sock(new ServerSocket(this)),
      trackerManager(nullptr),
      detectorTimer(new QTimer(this)),
      serverState(Idle),
      currentLevel(0),
      lastLevelChange(0),
      server(new QHttpServer)
{

    connect(this, SIGNAL(markersUpdated(vector<MarkerUpdate>)),
            this, SLOT(broadcastPositions(vector<MarkerUpdate>)));
    connect(sock, SIGNAL(errorOccurred(QString)),
            this, SIGNAL(socketError(QString)));
    connect(this, SIGNAL(markersUpdated(vector<MarkerUpdate>)),
            sock, SLOT(processUpdates()));
    connect(sock, SIGNAL(levelChanged(int)),
            this, SLOT(changeLevel(int)));
    connect(sock, SIGNAL(mirrorRotated(int, float, QTcpSocket*)),
            this, SLOT(setMirrorRotation(int, float, QTcpSocket*)));
    connect(sock, SIGNAL(clientConnected(QTcpSocket*)),
            this, SLOT(handleNewClient(QTcpSocket*)));
    connect(sock, SIGNAL(clientDisconnected(QTcpSocket*)),
            this, SLOT(clientDisconnected()));
    connect(sock, SIGNAL(arViewUpdated(int,cv::Point3f,cv::Point3f)),
            sock, SLOT(broadcastARViewUpdate(int,cv::Point3f,cv::Point3f)));
    connect(server, SIGNAL(newRequest(QHttpRequest*, QHttpResponse*)),
            this, SLOT(sendBoard(QHttpRequest*, QHttpResponse*)));


    // A single-shot Timer with an interval of 0 will
    // directly fire the timeout when control goes back
    // to the event thread. We use this structure to
    // allow signals events to be processed while
    // keeping the detector active as much as possible.
    detectorTimer->setInterval(0);
    detectorTimer->setSingleShot(true);
}

void ServerController::sendBoard(QHttpRequest* req, QHttpResponse* resp) {
    Q_UNUSED(req);

    resp->setHeader("Content-Type", "image/jpeg");
    resp->setHeader("Content-Length", QString::number(boardImageBytes.size()));
    resp->writeHead(200);
    resp->write(boardImageBytes);

    resp->end();
}

void ServerController::handleNewClient(QTcpSocket* newClient) {
    // If this is the first client, initialize the level change time
    if (lastLevelChange == 0) {
        lastLevelChange = time(NULL);
    }

    auto newClientFilter = [&](QTcpSocket* client) {
        return client->peerPort() == newClient->peerPort();
    };

    sock->broadcastLevelUpdate(currentLevel, 0, trackerManager->scaledBoardSize(), newClientFilter);

    for (auto& pair : mirrorRotations) {
        sock->broadcastRotationUpdate(pair.first, pair.second, newClientFilter);
    }

    // Update list of clients
    emit clientsChanged(sock->connections());
}

void ServerController::clientDisconnected() {
    // Update list of clients
    emit clientsChanged(sock->connections());
}

ServerController::~ServerController() {
    if (trackerManager != nullptr) {
        delete trackerManager;
    }
}

void ServerController::changeState(ServerState state) {
    this->serverState = state;
    emit stateChanged(state);
}

void ServerController::fatalError(const QString &message) {
    stopServer();
    emit fatalErrorOccurred(message);
}

void ServerController::startServer(quint16 port, int cameraDevice, cv::Size camSize, BoardDetectionApproach::Type boardDetectionApproach, bool requireEmptyBoard) {
    Q_ASSERT(serverState == Idle);
    Q_ASSERT(trackerManager == nullptr); // Otherwise we get a memory leak.
    changeState(Starting);

    this->requireEmptyBoard = requireEmptyBoard;

    // (Re)Initialize tracking
    trackerManager = new TrackerManager(cameraDevice, camSize, boardDetectionApproach);
    trackerManager->loadPatterns(MARKER_DIRECTORY, MARKER_COUNT);

    sock->setPortNumber(port);
    sock->start();
    detectorTimer->start();

    // Start detecting board
    connect(detectorTimer, SIGNAL(timeout()),
            this,          SLOT(detectBoard()));

    // Start server that broadcasts board image
    server->listen(port + 1);
}

void ServerController::stopServer() {
    Q_ASSERT(serverState == Started || serverState == Starting);
    changeState(Stopping);
    sock->stop();

    mirrorRotations.clear();
    currentLevel = 0;
    emit levelChanged(currentLevel);
}

void ServerController::changeLevel(int nextLevel) {
    if (nextLevel != currentLevel) {
        cv::Size2f boardSize(trackerManager->scaledBoardSize());
        int levelTime = time(NULL) - lastLevelChange;

        sock->broadcastLevelUpdate(nextLevel, levelTime, boardSize);

        currentLevel = nextLevel;
        lastLevelChange = time(NULL);

        emit levelChanged(nextLevel);
    }
}

void ServerController::setMirrorRotation(int id, float rotation, QTcpSocket* source) {
    mirrorRotations[id] = rotation;

    sock->broadcastRotationUpdate(id, rotation, [&](QTcpSocket* client) {
        return client->peerPort() != source->peerPort();
    });
}

void ServerController::detectBoard() {
    Q_ASSERT(trackerManager != nullptr);
    if (state() == Stopping) {
        delete trackerManager;
        trackerManager = nullptr;
        changeState(Idle);

        disconnect(detectorTimer, SIGNAL(timeout()),
                   this,          SLOT(detectBoard()));

        return;
    }
    Q_ASSERT(serverState == Starting);

    QPixmap result;
    bool boardLocated = trackerManager->locateBoard(result, true);

    // Show image to user
    emit imageReady(result);

    if (boardLocated) {
        // Save an image of the board without text overlay
        QPixmap board;
        auto updates = trackerManager->getMarkerUpdates(board, false);

        // If there are markers on the board, abort
        if (updates.size() == 0 || !requireEmptyBoard) {
            boardImageBytes.clear();
            QBuffer buffer(&boardImageBytes);
            buffer.open(QIODevice::WriteOnly);
            board.save(&buffer, "JPG");

            // When the board is found, we stop trying to locate
            // the board and start detecting markers instead.
            disconnect(detectorTimer, SIGNAL(timeout()),
                       this,          SLOT(detectBoard()));
            connect(detectorTimer,    SIGNAL(timeout()),
                    this,             SLOT(detectFrame()));
            changeState(Started);
        }
    }
    detectorTimer->start();
}

void ServerController::setDebugOverlay(bool enable) {
    showDebugOverlay = enable;
}

void ServerController::detectFrame() {
    // detectFrame should never be called when the server is not running.
    Q_ASSERT(serverState != Idle);
    Q_ASSERT(trackerManager != nullptr);
    if (state() == Starting) {
        changeState(Started);
    }

    if (state() == Started) {
        QPixmap result;
        vector<MarkerUpdate> markers = trackerManager->getMarkerUpdates(result, showDebugOverlay);

        emit markersUpdated(markers);
        emit imageReady(result);

        detectorTimer->start();

        emit fpsChanged(trackerManager->getUpdateRate());
    } else {
        changeState(Idle);
        delete trackerManager;
        trackerManager = nullptr;

        emit fpsChanged(-1);

        disconnect(detectorTimer,    SIGNAL(timeout()),
                this,             SLOT(detectFrame()));
    }
}

void ServerController::broadcastPositions(vector<MarkerUpdate> markers) {
    for(MarkerUpdate marker : markers) {
        broadcastPosition(marker);
    }
}

void ServerController::broadcastPosition(const MarkerUpdate& marker) {
    if (marker.type == MarkerUpdateType::REMOVE) {
        sock->broadcastDelete(marker.id);
    } else {
        // Scale marker positions based on their size for Meta 1 tracking
        sock->broadcastPositionUpdate(
                    marker.id,
                    trackerManager->scaledMarkerCoordinate(marker.position),
                    marker.rotation);
    }
}

} // namespace mirrors

