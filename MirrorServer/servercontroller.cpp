#include "servercontroller.h"
#include "serversocket.hpp"

namespace mirrors {

ServerController::ServerController(QObject *parent)
    : QObject(parent),
      sock(new ServerSocket(this)),
      boardDetector(nullptr),
      markerDetector(nullptr),
      markerTracker(nullptr),
      recognizer(nullptr),
      capture(nullptr),
      detectorTimer(new QTimer(this)),
      serverState(Idle)
{
    connect(this, SIGNAL(markersUpdated(vector<MarkerUpdate>)),
            this, SLOT(broadcastPositions(vector<MarkerUpdate>)));
    connect(detectorTimer, SIGNAL(timeout()),
            this,          SLOT(detectFrame()));
    connect(sock, SIGNAL(errorOccurred(QString)),
            this, SIGNAL(socketError(QString)));

    // A single-shot Timer with an interval of 0 will
    // directly fire the timeout when control goes back
    // to the event thread. We use this structure to
    // allow signals events to be processed while
    // keeping the detector active as much as possible.
    detectorTimer->setInterval(0);
    detectorTimer->setSingleShot(true);
}

ServerController::~ServerController() {
    if (capture != nullptr) {
        delete capture;
    }
}

void ServerController::changeState(ServerState state) {
    this->serverState = state;
    emit stateChanged(state);
}

void ServerController::startServer(quint16 port, int cameraDevice) {
    Q_ASSERT(serverState == Idle);
    Q_ASSERT(capture == nullptr); // Otherwise we get a memory leak.
    changeState(Starting);
    capture = new cv::VideoCapture(cameraDevice);

    sock->setPortNumber(port);
    sock->start();
    detectorTimer->start();
}

void ServerController::stopServer() {
    Q_ASSERT(serverState == Started);

    changeState(Stopping);
    sock->stop();
}

void ServerController::detectFrame() {
    // detectFrame should never be called when the server is not running.
    Q_ASSERT(serverState != Idle);
    Q_ASSERT(capture != nullptr);
    if (state() == Starting) {
        changeState(Started);
    }

    if (state() == Started) {
        Mat frame;
        capture->read(frame);
        vector<MarkerUpdate> markers = markerTracker->track(frame);
        emit markersUpdated(markers);

        boardDetector->extractBoard(frame);
        emit imageReady(frame);
        detectorTimer->start();
    } else {
        changeState(Idle);
        delete capture;
        capture = nullptr;
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
        sock->broadcastPositionUpdate(
                    marker.id,
                    marker.position,
                    marker.rotation);
    }
}

} // namespace mirrors

