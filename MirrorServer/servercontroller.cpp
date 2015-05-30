#include "servercontroller.h"
#include "serversocket.h"

namespace mirrors {

ServerController::ServerController(QObject *parent)
    : QObject(parent), sock(new ServerSocket(this)), det(nullptr),
      detectorTimer(new QTimer(this)), serverState(Idle)
{
    connect(this, SIGNAL(markersUpdated(vector<detected_marker>)),
            this, SLOT(broadcastPositions(vector<detected_marker>)));
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
    if (det != nullptr) {
        det->stop();
        delete det;
    }
}

void ServerController::changeState(ServerState state) {
    this->serverState = state;
    emit stateChanged(state);
}

void ServerController::startServer(quint16 port, int cameraDevice) {
    Q_ASSERT(serverState == Idle);
    Q_ASSERT(det == nullptr); // Otherwise we get a memory leak.
    changeState(Starting);
    det = new Detector(cameraDevice);
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
    if (state() == Starting) {
        changeState(Started);
    }

    if (state() == Started) {
        vector<detected_marker> markers = det->detect();
        emit markersUpdated(markers);
        emit imageReady(det->getLastFrame());
        detectorTimer->start();
    } else {
        Q_ASSERT(det != nullptr);
        changeState(Idle);
        delete det;
        det = nullptr;
    }
}

void ServerController::broadcastPositions(vector<detected_marker> markers) {
    for(detected_marker marker : markers) {
        broadcastPosition(marker);
    }
}

void ServerController::broadcastPosition(const detected_marker& marker) {
    if (marker.deleted) {
        sock->broadcastDelete(marker.id);
    } else {
        sock->broadcastPositionUpdate(
                    marker.id,
                    marker.position,
                    marker.rotation);
    }
}

} // namespace mirrors

