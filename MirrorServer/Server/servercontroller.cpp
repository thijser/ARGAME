#include "servercontroller.hpp"
#include "serversocket.hpp"
#include <iostream>

namespace mirrors {

ServerController::ServerController(QObject *parent)
    : QObject(parent),
      sock(new ServerSocket(this)),
      markerDetector(new MarkerDetector()),
      recognizer(new MarkerRecognizer()),
      capture(nullptr),
      detectorTimer(new QTimer(this)),
      serverState(Idle)
{
    // Load markers
    for (int i = 0; i < MARKER_COUNT; i++) {
        auto pattern = cv::imread("markers/" + std::to_string(i) + ".png", CV_LOAD_IMAGE_GRAYSCALE);
        recognizer->registerPattern(i, pattern);
    }

    connect(this, SIGNAL(markersUpdated(vector<MarkerUpdate>)),
            this, SLOT(broadcastPositions(vector<MarkerUpdate>)));
    connect(detectorTimer, SIGNAL(timeout()),
            this,          SLOT(detectBoard()));
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
    if (capture != nullptr) {
        delete capture;
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

void ServerController::startServer(quint16 port, int cameraDevice, cv::Size camSize, BoardDetectionApproach::BoardDetectionApproach boardDetectionApproach) {
    Q_ASSERT(serverState == Idle);
    Q_ASSERT(capture == nullptr); // Otherwise we get a memory leak.
    changeState(Starting);
    capture = new cv::VideoCapture(cameraDevice);
    if (!capture->isOpened()) {
        delete capture;
        capture = nullptr;
        fatalError(tr("Fatal error: could not open camera"));
    }

    capture->set(CV_CAP_PROP_FRAME_WIDTH,  camSize.width);
    capture->set(CV_CAP_PROP_FRAME_HEIGHT, camSize.height);

    cameraResolution.width = capture->get(CV_CAP_PROP_FRAME_WIDTH);
    cameraResolution.height = capture->get(CV_CAP_PROP_FRAME_HEIGHT);

    // Reconfigure board detector and tracker
    delete boardDetector;
    delete markerTracker;
    boardDetector = new BoardDetector(boardDetectionApproach);
    markerTracker = new MarkerTracker(*boardDetector, *markerDetector, *recognizer);

    sock->setPortNumber(port);
    sock->start();
    detectorTimer->start();
}

void ServerController::stopServer() {
    Q_ASSERT(serverState == Started || serverState == Starting);
    changeState(Stopping);
    sock->stop();
}

void ServerController::detectBoard() {
    Q_ASSERT(capture != nullptr);
    if (state() == Stopping) {
        delete capture;
        capture = nullptr;
        changeState(Idle);
        return;
    }
    Q_ASSERT(serverState == Starting);

    Mat frame;
    capture->read(frame);
    cv::imwrite("C:/Users/Alexander/Desktop/test.jpg", frame);
    if (boardDetector->locateBoard(frame)) {
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
    Q_ASSERT(capture != nullptr);
    if (state() == Starting) {
        changeState(Started);
    }

    if (state() == Started) {
        Mat frame;
        capture->read(frame);
        vector<MarkerUpdate> markers = markerTracker->track(frame);
        emit markersUpdated(markers);

        // Extract board view and render debug info on it
        Mat board = boardDetector->extractBoard(frame);

        if (showDebugOverlay) {
            drawDebugOverlay(*markerTracker, board, markers);
        }

        emit imageReady(board);

        detectorTimer->start();

        // Determine FPS if new second has started
        if (framesSecond != time(nullptr)) {
            emit fpsChanged(framesCount);

            framesSecond = time(nullptr);
            framesCount = 0;
        } else {
            framesCount++;
        }
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
        // Scale marker positions based on their size for Meta 1 tracking
        float scale = markerTracker->getMarkerScale();
        cv::Point2f scaledCoords(marker.position.x / scale, marker.position.y / scale);

        sock->broadcastPositionUpdate(
                    marker.id,
                    scaledCoords,
                    marker.rotation);
    }
}

void ServerController::drawDebugOverlay(MarkerTracker& tracker, Mat& board, const vector<MarkerUpdate>& markers) {
    float scale = tracker.getMarkerScale();
    cv::Size markerSize(scale * 8, scale * 8);

    for (MarkerUpdate update : markers) {
        // Draw border around recognized marker
        cv::RotatedRect rrect(update.position, markerSize, update.rotation);
        cv::Point2f rrectPoints[4];
        rrect.points(rrectPoints);

        cv::Point rectPoints[4];
        for (int i = 0; i < 4; i++) {
            rectPoints[i] = rrectPoints[i];
        }

        // Draw solid color rotated square on recognized marker
        cv::fillConvexPoly(board, rectPoints, 4, cv::Scalar(0, 0, 0, 128));

        // Draw centered text with recognized ID in it
        int baseLine;
        cv::Size textSize = cv::getTextSize(std::to_string(update.id), cv::FONT_HERSHEY_SIMPLEX, 1.2, 2, &baseLine);

        Point textPos = update.position + Point(-textSize.width / 2, textSize.height / 2);

        cv::putText(board, std::to_string(update.id), textPos, cv::FONT_HERSHEY_SIMPLEX, 1.2, cv::Scalar(255, 255, 255, 255), 2);
    }
}

} // namespace mirrors

