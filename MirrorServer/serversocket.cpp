/*
 * Copyright 2015, Delft University of Technology
 *
 * This software is licensed under the terms of the MIT license.
 * See http://opensource.org/licenses/MIT for the full license.
 *
 *
 */

#include "serversocket.h"

#include "netlink/socket.h"
#include "netlink/socket_group.h"

using namespace NL;

namespace mirrors {

ServerSocket::ServerSocket(uint serverPort) :
        sock(new Socket(serverPort)), keepGoing(true) {
}

ServerSocket::~ServerSocket() throw() {
    delete sock;
}

void ServerSocket::disconnect() {
    sock->disconnect();
    keepGoing = false;
}

void ServerSocket::run() {
    while (keepGoing) {
        clients.listen(2000);
    }
}

void ServerSocket::broadcastMessage(const void *buffer, int length) {
    for (uint i = 0; i < clients.size(); i++) {
        clients.get(i)->send(buffer, length);
    }
}

} // namespace mirrors
