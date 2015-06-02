#ifndef MIRRORS_SERVERCONTROLLER_H
#define MIRRORS_SERVERCONTROLLER_H

/**
 * @file servercontroller.h
 * @brief Defines the mirrors::ServerController class.
 */

#include <QTimer>
#include <vector>

#include "markertracker.hpp"
#include "markerdetector.hpp"
#include "markerrecognizer.hpp"
#include "boarddetector.hpp"

namespace mirrors {
using std::vector;

/// Horizontal capture resolution for camera.
const int CAMERA_WIDTH = 1600;

/// Vertical capture resolution for camera.
const int CAMERA_HEIGHT = 896;

/// Directory that contains marker images.
const std::string MARKER_DIRECTORY = "markers/";

/// Amount of markers.
const int MARKER_COUNT = 12;

class ServerSocket;

/**
 * @brief Controls main functionality of the Mirrors server.
 *
 * This class provides high-level signals and operations to the
 * user interface to operate and monitor the @c Detector and
 * @c ServerSocket instances.
 *
 * This class also sets up the required connections to forward
 * results from the Detector to the ServerSocket.
 */
class ServerController : public QObject {
    Q_OBJECT
    Q_DISABLE_COPY(ServerController)
public:
    /**
     * @brief Indicates the state of the ServerController.
     */
    enum ServerState {
        /// Indicates the Server is idle (not started).
        Idle = 0,
        /// Indicates the Server is starting.
        Starting,
        /// Indicates the Server is started and operational.
        Started,
        /// Indicates the Server is shutting down.
        Stopping
    };

    /**
     * @brief Creates a new ServerController.
     * @param parent - The parent of this QObject.
     */
    explicit ServerController(QObject *parent = 0);

    /**
     * @brief Stops the server and deletes this ServerController.
     */
    virtual ~ServerController();

    /**
     * @brief The current state of the ServerController.
     * @return The current ServerState.
     */
    ServerState state() const { return serverState; }

signals:
    /**
     * @brief Signal emitted when the board has been detected.
     */
    void boardFound();

    /**
     * @brief Signal emitted whenever an image frame is processed.
     * @param image - The processed image.
     */
    void imageReady(const cv::Mat& image);

    /**
     * @brief Signal emitted whenever marker positions have changed.
     * @param markers - The changed markers with their new positions.
     */
    void markersUpdated(vector<MarkerUpdate> markers);

    /**
     * @brief Signal emitted whenever the state of this server changes.
     * @param state - The new ServerState.
     */
    void stateChanged(ServerState state);

    /**
     * @brief Signal emitted whenever an error occurs in a Socket.
     * @param message - The QString describing the error.
     */
    void socketError(QString message);

protected slots:
    /**
     * @brief Changes the current ServerState.
     *
     * This updates the value returned by @c state(), as well as
     * emits the @c stateChanged(ServerState) signal.
     *
     * @param state - The new ServerState
     */
    void changeState(ServerState state);
public slots:
    /**
     * @brief Starts this ServerController.
     * @param port - The port number to bind to.
     */
    void startServer(quint16 port, int cameraDevice = -1);

    /**
     * @brief Stops this ServerController.
     */
    void stopServer();

    /**
     * @brief Attempts to detect the board
     */
    void detectBoard();

    /**
     * @brief Detects a single frame using a detector instance.
     */
    void detectFrame();

    /**
     * @brief Broadcasts the detected markers to all connected clients.
     * @param markers - The marker data to send.
     */
    void broadcastPositions(vector<MarkerUpdate> markers);

    /**
     * @brief Broadcasts the detected marker to all connected clients.
     * @param marker - The marker data to send.
     */
    void broadcastPosition(const MarkerUpdate& marker);

private:
    /// The ServerSocket used to send messages
    ServerSocket *sock;

    /// The BoardDetector used to find the playing area.
    BoardDetector *boardDetector;

    /// The MarkerDetector instance used to find markers.
    MarkerDetector *markerDetector;

    /// The MarkerRecognizer instance used to recognize marker patterns.
    MarkerRecognizer *recognizer;

    /// The MarkerTracker instance used to follow markers.
    MarkerTracker *markerTracker;

    /// The OpenCV VideoCapture object used to get video frames.
    cv::VideoCapture *capture;

    /// The QTimer used for scheduling marker detection
    QTimer *detectorTimer;

    /// Flag indicating if the server is running.
    ServerState serverState;
};

} // namespace mirrors

#endif // MIRRORS_SERVERCONTROLLER_H
