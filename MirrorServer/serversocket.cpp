/*
 * Copyright 2015, Delft University of Technology
 *
 * This software is licensed under the terms of the MIT license.
 * See http://opensource.org/licenses/MIT for the full license.
 *
 *
 */

#include "serversocket.h"

#include <iostream>
using namespace std;

#include "netlink/socket.h"
#include "netlink/socket_group.h"
using namespace NL;

namespace mirrors {

namespace {
    // Helper function that writes arbitrary types
    // to a char array.
    template<typename T>
    void writeValue(T f, char *buf, int offset) {
        char* bytes = reinterpret_cast<char*>(&f);
        for (unsigned long i = 0; i < sizeof(T); i++) {
            buf[offset + i] = bytes[i];
        }
    }

    // Command that is called on accept
    class AcceptCommand : public SocketGroupCmd {
    public:
        virtual void exec(Socket *socket, SocketGroup *group, void*) {
            cout << "Accepted connection from " << socket->hostFrom() << endl;
            group->add(socket);
        }
    };
}

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
    cout << "Server started (listening on port " << sock->portFrom() << ")" << endl;
    AcceptCommand accept;
    clients.setCmdOnAccept(&accept);
    while (keepGoing) {

        Socket *client = sock->accept();
        clients.add(client);

        broadcastPositionUpdate(1234, 0.25f, 0.75f, 200000);
    }
    cout << "Server stopped" << endl;
}

void ServerSocket::broadcastMessage(const void *buffer, int length) {
    for (uint i = 0; i < clients.size(); i++) {
        clients.get(i)->send(buffer, length);
    }
}

void ServerSocket::broadcastPositionUpdate(uint32_t id, float x, float y, uint64_t timestamp) {
    char buffer[20];
    writeValue(x, buffer, 0);
    writeValue(y, buffer, 4);
    writeValue(id, buffer, 8);
    writeValue(timestamp, buffer, 12);

    broadcastMessage(buffer, 20);
}

} // namespace mirrors
