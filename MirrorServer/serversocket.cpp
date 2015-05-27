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

#include "netlink/socket.h"
#include "netlink/socket_group.h"

namespace mirrors {

using namespace std;
using namespace NL;

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
}

ServerSocket::ServerSocket(uint16_t serverPort) :
        port(serverPort), sock(NULL), keepGoing(true) {
}

ServerSocket::~ServerSocket() throw() {
    if (sock != NULL) {
        delete sock;
    }
}

void ServerSocket::disconnect() {
    keepGoing = false;
}

void ServerSocket::run() throw (NL::Exception, std::logic_error) {
    socketMutex.lock();
    if (sock != NULL) {
        socketMutex.unlock();
        throw std::logic_error("ServerSocket::run - Cannot run multiple times");
    }

    sock = new Socket(port);
    socketMutex.unlock();
    clog << "Server started (listening on port " << sock->portFrom() << ")" << endl;
    while (keepGoing) {
        try {
            socketMutex.lock();
            Socket *client = sock->accept(2000);
            socketMutex.unlock();
            if (client != NULL) {
                clog << "Added client: " << client->hostTo() << endl;
                clients.add(client);
            }
        } catch (const NL::Exception &ex) {
            socketMutex.unlock();
            clog << "Exception in Socket accept: " << ex.msg() << endl;
        }
    }

    socketMutex.lock();
    delete sock;
    sock = NULL;
    socketMutex.unlock();
    clog << "Server stopped" << endl;
}

void ServerSocket::broadcastMessage(const void *buffer, int length) {
    for (uint16_t i = 0; i < clients.size(); i++) {
        Socket *client = clients.get(i);
        try {
            client->send(buffer, length);
        } catch (const NL::Exception &ex) {
            clog << "Removed client: " << client->hostTo() << " (reason: " << ex.msg() << ")" << endl;
            clients.remove(i);
        }
    }
}

void ServerSocket::broadcastPositionUpdate(uint32_t id, float x, float y, float rotation, uint64_t timestamp) {
    char buffer[24];
    writeValue(x, buffer, 0);
    writeValue(y, buffer, 4);
    writeValue(rotation, buffer, 8);
    writeValue(id, buffer, 12);
    writeValue(timestamp, buffer, 16);

    broadcastMessage(buffer, 24);
}

} // namespace mirrors
