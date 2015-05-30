#ifndef MIRRORS_SERVERCONTROLLER_H
#define MIRRORS_SERVERCONTROLLER_H

/**
 * @file servercontroller.h
 * @brief Defines the mirrors::ServerController class.
 */

#include <QTimer>
#include <vector>

#include "detector.hpp"

namespace mirrors {
using std::vector;

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
     * @brief Signal emitted whenever an image frame is processed.
     * @param image - The processed image.
     */
    void imageReady(const cv::Mat& image);

    /**
     * @brief Signal emitted whenever marker positions have changed.
     * @param markers - The changed markers with their new positions.
     */
    void markersUpdated(vector<detected_marker> markers);

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
     * @brief Detects a single frame using a detector instance.
     */
    void detectFrame();

    /**
     * @brief Broadcasts the detected markers to all connected clients.
     * @param markers - The marker data to send.
     */
    void broadcastPositions(vector<detected_marker> markers);

    /**
     * @brief Broadcasts the detected marker to all connected clients.
     * @param marker - The marker data to send.
     */
    void broadcastPosition(const detected_marker& marker);

private:
    /// The ServerSocket used to send messages
    ServerSocket *sock;

    /// The detector instance used to find markers.
    Detector *det;

    /// The QTimer used for scheduling marker detection
    QTimer *detectorTimer;

    /// Flag indicating if the server is running.
    ServerState serverState;
};

} // namespace mirrors

#endif // MIRRORS_SERVERCONTROLLER_H
