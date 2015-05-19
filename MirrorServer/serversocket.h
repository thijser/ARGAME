#ifndef MIRRORS_SERVERSOCKET_H
#define MIRRORS_SERVERSOCKET_H

#include <vector>

#include "netlink/socket_group.h"

namespace NL {
  class Socket;
  class SocketGroup;
}

using namespace NL;

namespace mirrors {

/**
 * @brief Server implementation using NetLink sockets.
 */
class ServerSocket {
private:
    Socket *sock;
    SocketGroup clients;
public:
    /**
     * @brief Creates a new server Socket that connects to the given port.
     * @param serverPort - The port of the server Socket.
     */
    explicit ServerSocket(unsigned serverPort);

    virtual ~ServerSocket() throw();

    /**
     * @brief The NetLink server Socket
     * @return The NetLink Socket being used.
     */
    Socket* serverSocket() const { return sock; }

    /**
     * @brief The SocketGroup containing all connected clients.
     * @return The SocketGroup.
     */
    const SocketGroup* connectedClients() const { return &clients; }

    /**
     * @brief Broadcasts a message to all clients.
     *
     * @param buffer - The buffer with the data to send.
     * @param length - The length of the data, must be positive.
     */
    void broadcastMessage(char *buffer, int length);
};

} // namespace mirrors

#endif // MIRRORS_SERVERSOCKET_H
