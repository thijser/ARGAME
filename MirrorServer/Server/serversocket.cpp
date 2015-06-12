#include "serversocket.hpp"

#include <QTcpServer>
#include <QTcpSocket>
#include <QDataStream>

#include <iostream>

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
    pingTimer->start();
    emit started();
}

void ServerSocket::stop() {
    sock->close();
    pingTimer->stop();
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


void ServerSocket::broadcastRotationUpdate(int id, float rotation) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
    stream << (qint8) 3
           << (qint32) id
           << rotation;
    broadcastBytes(bytes);
}

void ServerSocket::broadcastLevelUpdate(int levelIndex, cv::Size2f boardSize) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
    stream << (qint8) 4
           << (qint32) levelIndex
           << boardSize.width
           << boardSize.height;

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

void ServerSocket::processUpdates() {
    foreach (QTcpSocket *sock, clients) {
        processUpdates(sock);
    }
}

void ServerSocket::processUpdates(QTcpSocket *client) {
    Q_ASSERT(client != nullptr);
    qint8 tag = -1;
    bool readOk = client->read((char*) &tag, 1) == 1;

    while (readOk) {
        switch (tag) {
        case 3: // Rotation Update
            readRotationUpdate(client);
            break;
        case 4: // Level Update
            readLevelUpdate(client);
            break;
        default:
            qDebug() << "Received message with unknown tag:" << (int) tag;
        }

        readOk = client->read((char*) &tag, 1) == 1;
    }
}

void ServerSocket::readRotationUpdate(QTcpSocket *client) {
    Q_ASSERT(client != nullptr);
    if (client->bytesAvailable() >= 8) {
        QByteArray data = client->read(8);
        if (data.length() != 8) {
            return;
        }

        qint32 id;
        float rotation;
        QDataStream stream(&data, QIODevice::ReadOnly);
        stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
        stream >> id;
        stream >> rotation;
        broadcastRotationUpdate(id, rotation);
    }
}

void ServerSocket::readLevelUpdate(QTcpSocket *client) {
    Q_ASSERT(client != nullptr);
    if (client->bytesAvailable() >= 12) {
        QByteArray data = client->read(12);
        if (data.length() != 12) {
            return;
        }

        qint32 levelIndex;
        QDataStream stream(&data, QIODevice::ReadOnly);
        stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
        stream >> levelIndex;
        emit levelChanged(levelIndex);
    }
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
