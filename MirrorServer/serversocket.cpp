#include "serversocket.h"

#include "netlink/socket.h"
#include "netlink/socket_group.h"

using namespace NL;

namespace mirrors {

ServerSocket::ServerSocket(uint serverPort) :
        sock(new Socket(serverPort)) {
}

ServerSocket::~ServerSocket() throw() {
    delete sock;
}

void ServerSocket::broadcastMessage(char *buffer, int length) {
    for (uint i = 0; i < clients.size(); i++) {
        clients.get(i)->send(buffer, length);
    }
}

} // namespace mirrors
