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

#include "netlink/socket_group.h"

namespace NL {
  class Socket;
}

using namespace NL;

namespace mirrors {

/**
 * @brief Server implementation using NetLink sockets.
 */
class ServerSocket {
private:
    /// The Socket that clients can connect to.
    Socket *sock;

    /// The SocketGroup maintaining the connected clients.
    SocketGroup clients;

    /// Flag indicating if the server should still be running.
    bool keepGoing;
public:
    /**
     * @brief Creates a new server Socket that connects to the given port.
     * @param serverPort - The port of the server Socket.
     */
    explicit ServerSocket(uint serverPort);

    /**
     * @brief Deletes the Socket.
     */
    virtual ~ServerSocket() throw();

    /**
     * @brief Disconnects the server Socket.
     */
    void disconnect();

    /**
     * @brief Runs the server until disconnected.
     *
     * This function will only return after the Socket is disconnected.
     */
    void run();

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
};

} // namespace mirrors

#endif // MIRRORS_SERVERSOCKET_H
