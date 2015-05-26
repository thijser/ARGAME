/*
 * Copyright 2015, Delft University of Technology
 *
 * This software is licensed under the terms of the MIT license.
 * See http://opensource.org/licenses/MIT for the full license.
 *
 *
 */

#ifndef MIRRORS_SERVERSOCKET_H
#define MIRRORS_SERVERSOCKET_H

#include <mutex>

#include "netlink/socket.h"
#include "netlink/socket_group.h"

namespace mirrors {

using namespace NL;

/**
 * @brief Server implementation using NetLink sockets.
 */
class ServerSocket {
private:
    /// The port the server listens on.
    uint16_t port;

    /// The Socket that clients can connect to.
    Socket *sock;

    /// The SocketGroup maintaining the connected clients.
    SocketGroup clients;

    /// Flag indicating if the server should still be running.
    bool keepGoing;

    /// The mutex used for locking the Socket.
    std::mutex socketMutex;

    // Disable copying of ServerSockets.
    ServerSocket(const ServerSocket&) = delete;
    ServerSocket& operator=(const ServerSocket&) = delete;
public:
    /**
     * @brief Creates a new server Socket that connects to the given port.
     * @param serverPort - The port of the server Socket.
     */
    explicit ServerSocket(uint16_t serverPort);

    /**
     * @brief Closes and deletes the Socket.
     */
    virtual ~ServerSocket() throw();

    /**
     * @brief Disconnects the server Socket.
     */
    void disconnect();

    /**
     * @brief Tests whether this server is running.
     * @return True if this server is running, false otherwise.
     */
    bool isRunning() const { return sock != NULL; }

    /**
     * @brief Runs the server until disconnected.
     *
     * This function will only return after the Socket is disconnected.
     * @throws NL::Exception    - If an exception occurred in the NetLink Socket.
     * @throws std::logic_error - If this server is already running.
     */
    void run() throw (NL::Exception, std::logic_error);

    /**
     * @brief The amount of connections this Server currently has.
     * @return The amount of connections.
     */
    int connectionCount() const { return clients.size(); }

    /**
     * @brief Broadcasts a message to all clients.
     *
     * @param buffer - The buffer with the data to send.
     * @param length - The length of the data, must be positive.
     */
    void broadcastMessage(const void *buffer, int length);

    /**
     * @brief Broadcasts a position update to all clients.
     * @param id        - The id of the object.
     * @param x         - The x coordinate.
     * @param y         - The y coordinate.
     * @param rotation  - The rotation of the object.
     * @param timestamp - The timestamp of the update.
     */
    void broadcastPositionUpdate(uint32_t id, float x, float y, float rotation, uint64_t timestamp);

    /**
     * @brief Enables the use of Sockets.
     *
     * This is required to be called on Windows (for initializing WSA),
     * but doesn't do any harm on other systems.
     *
     * This static function delegates to @c NL::init()
     */
    static void initialize() throw(NL::Exception) { NL::init(); }
};

} // namespace mirrors

#endif // MIRRORS_SERVERSOCKET_H
