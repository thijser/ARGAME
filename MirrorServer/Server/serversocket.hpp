#ifndef SERVERSOCKET_H
#define SERVERSOCKET_H

/**
 * @file serversocket.h
 * @brief Defines the mirrors::ServerSocket class.
 */

#include <QTimer>

class QTcpServer;
class QTcpSocket;

namespace mirrors {

/**
 * @brief Mirrors TCP Server class.
 *
 * This class manages the connections to clients and
 * provides access to those clients.
 */
class ServerSocket : public QObject
{
    Q_OBJECT
    Q_DISABLE_COPY(ServerSocket)
private:
    /// The internal QTcpServer that listens for incoming connections.
    QTcpServer *sock;

    /// The QList of connected clients.
    QList<QTcpSocket*> clients;

    /// The QTimer object that periodically sends keep-alive (ping) messages to clients.
    QTimer *pingTimer;

    /// The port to which the server is bound.
    quint16 port;
public:
    /**
     * @brief Initializes a new instance of the @c mirrors::ServerSocket class.
     * @param parent - The parent of this QObject instance, may be NULL.
     */
    explicit ServerSocket(QObject *parent = 0);

    /**
     * @brief The port number used by this ServerSocket.
     * @return The port number.
     */
    quint16 portNumber() const { return port; }

    /**
     * @brief The QTcpServer socket used to listen for incoming connections.
     * @return The QTcpServer used by this ServerSocket.
     */
    QTcpServer* socket() const { return sock; }

    /**
     * @brief Returns a QList with the active clients.
     * @return The active connections to this server.
     */
    QList<QTcpSocket*> connections() const { return clients; }

    /**
     * @brief Indicates if the server is currently started.
     * @return True if the server is started, false otherwise.
     */
    bool isStarted() const;

    /**
     * @brief Sets the port number to use for the server.
     *
     * This setter function should only be called when the
     * server is not listening.
     *
     * @param portNum - The port number.
     */
    void setPortNumber(quint16 portNum);
signals:
    /**
     * @brief Signal emitted when the server is started.
     */
    void started();

    /**
     * @brief Signal emitted when the server is stopped.
     */
    void stopped();

    /**
     * @brief Signal emitted every time a client connects to the server.
     * @param client - The client that connected.
     */
    void clientConnected(QTcpSocket *client);

    /**
     * @brief Signal emitted every time a client diconnects from the server.
     * @param client - The client that got disconnected.
     */
    void clientDisconnected(QTcpSocket *client);

    /**
     * @brief Signal emitted when an internal server error occurs.
     *
     * This signal often indicates critical failures, and should be
     * monitored at all times to detect possible errors in the socket
     * connection.
     *
     * @param message - A QString describing the error.
     */
    void errorOccurred(QString message);

public slots:
    /**
     * @brief Starts the Server.
     *
     * If the server fails to start, the @c errorOccurred(QString)
     * signal is emitted indicating the error. If the server started
     * successfully, the @c started() signal is emitted.
     */
    void start();

    /**
     * @brief Stops the Server.
     *
     * When this function returns, the server may not have stopped directly.
     * The @c stopped() signal is emitted when the server is actually stopped.
     */
    void stop();

    /**
     * @brief Disconnects the given QTcpSocket from this Server.
     *
     * If the given socket was connected to this server, the
     * @c clientDisconnected(QTcpSocket*) signal is emitted with
     * the argument QTcpSocket as parameter.
     *
     * @param client - The QTcpSocket to disconnect.
     */
    void disconnect(QTcpSocket *client);

    /**
     * @brief Sends the given QByteArray to all clients.
     * @param bytes - The QByteArray with the data to send.
     */
    void broadcastBytes(QByteArray bytes);

    /**
     * @brief Sends a PositionUpdate message to all clients.
     * @param id       - The marker ID.
     * @param position - The position of the marker.
     * @param rotation - The rotation of the marker.
     */
    void broadcastPositionUpdate(int id, cv::Point2f position, float rotation);

    /**
     * @brief Sends a Delete message to all clients.
     * @param id - The marker ID.
     */
    void broadcastDelete(int id);

    /**
     * @brief Sends a Ping message to all clients.
     */
    void broadcastPing();

private slots:
    /**
     * @brief Attempts to accept a pending connection.
     *
     * If an error occurs, the @c errorOccurred(QString) signal is
     * emitted. Otherwise, the pending connection is accepted and
     * the @c clientConnected(QTcpSocket*) signal is emitted with
     * the accepted connection.
     */
    void newConnection();

    /**
     * @brief Handles an internal socket error, if any.
     *
     * This slot emits the @c errorOccurred(QString) signal
     * indicating the error.
     *
     * If no error has actually occurred, a previous error
     * message may be re-emitted through the
     * @c errorOccurred(QString) signal.
     */
    void handleError();

    /**
     * @brief Handles an error in a client QTcpSocket.
     *
     * This assumes the QTcpSocket that caused the error
     * is the sender of the signal that invoked this slot.
     *
     * If the sender is no QTcpSocket instance, this function
     * has no effect.
     */
    void handleClientError();
};

}

#endif // SERVERSOCKET_H