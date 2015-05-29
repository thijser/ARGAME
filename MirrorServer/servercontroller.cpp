#include "servercontroller.h"
#include "serversocket.h"

namespace mirrors {

ServerController::ServerController(QObject *parent)
    : QObject(parent), sock(new ServerSocket(this)), det(new detector()),
      detectorTimer(new QTimer(this)), serverState(Idle)
{
    connect(this, SIGNAL(markersUpdated(vector<detected_marker>)),
            this, SLOT(broadcastUpdates(vector<detected_marker>)));
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

void ServerController::startServer(int port) {
    changeState(Starting);
    sock->setPortNumber(port);
    sock->start();
    detectorTimer->start();
}

void ServerController::stopServer() {
    changeState(Stopping);
    sock->stop();
    det->stop();
    detectorTimer->stop();
}

void ServerController::detectFrame() {
    if (state() == Starting) {
        changeState(Started);
    }

    vector<detected_marker> markers = det->detect();
    emit markersUpdated(markers);
    emit imageReady(det->getLastFrame());

    // Schedule the next invocation.
    if (state() == Started) {
        detectorTimer->start();
    } else {
        changeState(Idle);
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
                    marker.position.x,
                    marker.position.y,
                    marker.rotation);
    }
}

} // namespace mirrors

