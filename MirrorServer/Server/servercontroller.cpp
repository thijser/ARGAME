#include "servercontroller.hpp"
#include "serversocket.hpp"
#include <iostream>

namespace mirrors {

const std::string boardLocatingInstructions = "Position the camera to view the entire board.";

ServerController::ServerController(QObject *parent)
    : QObject(parent),
      sock(new ServerSocket(this)),
      trackerManager(nullptr),
      detectorTimer(new QTimer(this)),
      serverState(Idle)
{

    connect(this, SIGNAL(markersUpdated(vector<MarkerUpdate>)),
            this, SLOT(broadcastPositions(vector<MarkerUpdate>)));
    connect(sock, SIGNAL(errorOccurred(QString)),
            this, SIGNAL(socketError(QString)));
    connect(this, SIGNAL(markersUpdated(vector<MarkerUpdate>)),
            sock, SLOT(processUpdates()));

    // A single-shot Timer with an interval of 0 will
    // directly fire the timeout when control goes back
    // to the event thread. We use this structure to
    // allow signals events to be processed while
    // keeping the detector active as much as possible.
    detectorTimer->setInterval(0);
    detectorTimer->setSingleShot(true);
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

void ServerController::startServer(quint16 port, int cameraDevice, cv::Size camSize, BoardDetectionApproach::Type boardDetectionApproach) {
    Q_ASSERT(serverState == Idle);
    Q_ASSERT(trackerManager == nullptr); // Otherwise we get a memory leak.
    changeState(Starting);

    // (Re)Initialize tracking
    trackerManager = new TrackerManager(cameraDevice, camSize, boardDetectionApproach);
    trackerManager->loadPatterns(MARKER_DIRECTORY, MARKER_COUNT);

    sock->setPortNumber(port);
    sock->start();
    detectorTimer->start();

    // Start detecting board
    connect(detectorTimer, SIGNAL(timeout()),
            this,          SLOT(detectBoard()));
}

void ServerController::stopServer() {
    Q_ASSERT(serverState == Started || serverState == Starting);
    changeState(Stopping);
    sock->stop();
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

    Mat result;
    bool boardLocated = trackerManager->locateBoard(result, true);

    // Show image to user
    emit imageReady(result);

    if (boardLocated) {
        // When the board is found, we stop trying to locate
        // the board and start detecting markers instead.
        disconnect(detectorTimer, SIGNAL(timeout()),
                   this,          SLOT(detectBoard()));
        connect(detectorTimer,    SIGNAL(timeout()),
                this,             SLOT(detectFrame()));
        changeState(Started);
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
        Mat result;
        vector<MarkerUpdate> markers = trackerManager->getMarkerUpdates(result, showDebugOverlay);

        emit markersUpdated(markers);
        emit imageReady(result);

        detectorTimer->start();

        emit fpsChanged(trackerManager->getUpdateRate());
    } else {
        changeState(Idle);
        delete trackerManager;
        trackerManager = nullptr;

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
                    trackerManager->scaledMarkerCoordinate(marker),
                    marker.rotation);
    }
}

} // namespace mirrors

