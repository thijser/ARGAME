#include "serversocket.h"

#include <QTcpServer>
#include <QTcpSocket>

namespace Mirrors {

namespace {
    template <typename T>
    QByteArray toRawBytes(T value) {
        const char *bytes = reinterpret_cast<const char*>(&value);
        return QByteArray(bytes, sizeof(T));
    }
}

ServerSocket::ServerSocket(QObject *parent)
    : QObject(parent), sock(new QTcpServer(this)), pingTimer(new QTimer(this))
{
    connect(sock, SIGNAL(newConnection()),
            this, SLOT(newConnection()));
    connect(sock, SIGNAL(acceptError(QAbstractSocket::SocketError)),
            this, SLOT(handleError()));

    pingTimer->setInterval(1000);
    connect(pingTimer, SIGNAL(timeout()),
            this,      SLOT(broadcastPing()));
}

bool ServerSocket::isStarted() const {
    return sock->isListening();
}

void ServerSocket::setPortNumber(quint16 portNum) {
    Q_ASSERT(!sock->isListening());
    port = portNum;
}

void ServerSocket::start() {
    sock->listen(QHostAddress::Any, 23369);
    emit started();
}

void ServerSocket::stop() {
    sock->close();
    emit stopped();
}

void ServerSocket::disconnect(QTcpSocket *client) {
    if (clients.contains(client)) {
        clients.removeOne(client);
        if (client->isOpen()) {
            client->close();
        }
        emit clientDisconnected(client);
        client->deleteLater();
    }
}

void ServerSocket::broadcastBytes(QByteArray bytes) {
    foreach (QTcpSocket *client, clients) {
        client->write(bytes);
    }
}

void ServerSocket::broadcastPositionUpdate(int id, float x, float y, float rotation) {
    QByteArray bytes;
    bytes.append((char)0)
         .append(toRawBytes(x))
         .append(toRawBytes(y))
         .append(toRawBytes(rotation))
         .append(toRawBytes(id));
    broadcastBytes(bytes);
}

void ServerSocket::broadcastDelete(int id) {
    QByteArray bytes;
    bytes.append((char)1)
         .append(toRawBytes(id));
    broadcastBytes(bytes);
}

void ServerSocket::broadcastPing() {
    broadcastBytes(QByteArray(1, 2));
}

void ServerSocket::newConnection() {
    QTcpSocket *client = sock->nextPendingConnection();
    clients.append(client);
    emit clientConnected(client);
}

void ServerSocket::handleError() {
    emit errorOccurred(sock->errorString());
}

void ServerSocket::handleClientError() {
    QTcpSocket *client = qobject_cast<QTcpSocket*>(sender());
    if (client != nullptr) {
        disconnect(client);
    }
}

}
