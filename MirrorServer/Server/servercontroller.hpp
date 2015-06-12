#ifndef MIRRORS_SERVERCONTROLLER_H
#define MIRRORS_SERVERCONTROLLER_H

/**
 * @file servercontroller.h
 * @brief Defines the mirrors::ServerController class.
 */

#include <QTimer>
#include <vector>
#include <ctime>

#include "markertracker.hpp"
#include "markerdetector.hpp"
#include "markerrecognizer.hpp"
#include "boarddetector.hpp"

namespace mirrors {
using std::vector;

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

    /**
     * @brief The current camera resolution.
     * @return The camera resolution.
     */
    cv::Size resolution() const { return cameraResolution; }

    /**
     * @brief Set if the debug overlay should be enabled.
     * @param enable - True if debug overlay should be shown.
     */
    void setDebugOverlay(bool enable);

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
     * @brief Signal emitted whenever a new framerate has been determined.
     * @param fps - New framerate.
     */
    void fpsChanged(int fps);

    /**
     * @brief Signal emitted whenever the level changed.
     * @param level - The new level.
     */
    void levelChanged(int level);

    /**
     * @brief Signal emitted whenever an error occurs in a Socket.
     * @param message - The QString describing the error.
     */
    void socketError(QString message);

    /**
     * @brief Signal emitted when the server encounters a fatal error.
     *
     * After this signal is emitted, it may be assumed the server is in the
     * @c Idle state.
     *
     * @param errorMessage - a localized QString describing the error.
     */
    void fatalErrorOccurred(QString errorMessage);

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

    /**
     * @brief Stops the server and emits the @c fatalErrorOccurred(QString) signal.
     * @param message - A localized message describing the error.
     */
    void fatalError(const QString& message);
public slots:
    /**
     * @brief Starts this ServerController.
     * @param port         - The port number to bind to.
     * @param cameraDevice - The camera to use.
     * @param camSize      - The desired camera resolution.
     */
    void startServer(quint16 port, int cameraDevice = -1, cv::Size camSize = cv::Size(640,480), BoardDetectionApproach::BoardDetectionApproach boardDetectionApproach = BoardDetectionApproach::RED_MARKERS);

    /**
     * @brief Stops this ServerController.
     */
    void stopServer();

    /**
     * @brief Sends a level update message to all clients.
     * @param nextLevel - The next level index.
     */
    void changeLevel(int nextLevel);

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
    BoardDetector *boardDetector = nullptr;

    /// The MarkerDetector instance used to find markers.
    MarkerDetector *markerDetector;

    /// The MarkerRecognizer instance used to recognize marker patterns.
    MarkerRecognizer *recognizer;

    /// The MarkerTracker instance used to follow markers.
    MarkerTracker *markerTracker = nullptr;

    /// The OpenCV VideoCapture object used to get video frames.
    cv::VideoCapture *capture;

    /// The QTimer used for scheduling marker detection
    QTimer *detectorTimer;

    /// Flag indicating if the server is running.
    ServerState serverState;

    /// The actual camera resolution.
    cv::Size cameraResolution;

    /// Second for which frames are counted.
    time_t framesSecond;

    /// Amount of frames that have been processed this second.
    int framesCount;

    /// Boolean indicating if debug overlay should be shown.
    bool showDebugOverlay = true;

    /**
     * @brief Draw instructions for locating the board on top of the camera image.
     * @param frame - Camera image to draw instructions to.
     */
    static void drawBoardLocatingInstructions(Mat& frame);

    /**
     * @brief Draw the marker positions and IDs on top of the board image.
     * @param board - Board image.
     * @param markers - Marker positions and other info.
     */
    static void drawDebugOverlay(MarkerTracker& tracker, Mat& board, const vector<MarkerUpdate>& markers);
};

} // namespace mirrors

#endif // MIRRORS_SERVERCONTROLLER_H
