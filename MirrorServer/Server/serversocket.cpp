#include "serversocket.hpp"

#include <QTcpServer>
#include <QTcpSocket>
#include <QDataStream>

namespace mirrors {

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

void ServerSocket::broadcastPositionUpdate(int id, cv::Point2f position, float rotation) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
    stream << (qint8) 0
           << position.x
           << position.y
           << rotation
           << (qint32) id;

    broadcastBytes(bytes);
}

void ServerSocket::broadcastDelete(int id) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream << (qint8) 1
           << (qint32) id;
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
