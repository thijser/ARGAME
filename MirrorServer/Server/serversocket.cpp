#include "serversocket.hpp"

#include <QTcpServer>
#include <QTcpSocket>
#include <QDataStream>
#include <opencv2/core.hpp>

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

void ServerSocket::broadcastBytes(QByteArray bytes, std::function<bool(QTcpSocket*)> filter) {
    foreach (QTcpSocket *client, clients) {
        if (filter(client)) {
            client->write(bytes);
        }
    }
}

void ServerSocket::broadcastPositionUpdate(int id, cv::Point2f position, float rotation, std::function<bool(QTcpSocket*)> filter) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
    stream << (qint8) 0
           << position.x
           << position.y
           << rotation
           << (qint32) id;

    broadcastBytes(bytes, filter);
}


void ServerSocket::broadcastRotationUpdate(int id, float rotation, std::function<bool(QTcpSocket*)> filter) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
    stream << (qint8) 3
           << (qint32) id
           << rotation;

    broadcastBytes(bytes, filter);
}

void ServerSocket::broadcastLevelUpdate(int levelIndex, int levelTime, cv::Size2f boardSize, std::function<bool(QTcpSocket*)> filter) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
    stream << (qint8) 4
           << (qint32) levelIndex
           << (qint32) levelTime
           << boardSize.width
           << boardSize.height;

    broadcastBytes(bytes, filter);
}

void ServerSocket::broadcastDelete(int id, std::function<bool(QTcpSocket*)> filter) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream << (qint8) 1
           << (qint32) id;
    broadcastBytes(bytes, filter);
}

void ServerSocket::broadcastPing(std::function<bool(QTcpSocket*)> filter) {
    broadcastBytes(QByteArray(1, 2), filter);
}

void ServerSocket::broadcastARViewUpdate(int id, cv::Point3f position, cv::Point3f rotation) {
    QByteArray bytes;
    QDataStream stream(&bytes, QIODevice::WriteOnly);
    stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
    stream << (qint8) 5
           << (qint32) id
           << position.x
           << position.y
           << position.z
           << rotation.x
           << rotation.y
           << rotation.z;

    qDebug() << "AR View:" << id
             << "Position:" << position.x << position.y << position.z
             << "Rotation:" << rotation.x << rotation.y << rotation.z;

    broadcastBytes(bytes);
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
        case 5: // AR View Update
            readARViewUpdate(client);
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
        emit mirrorRotated(id, rotation, client);
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

void ServerSocket::readARViewUpdate(QTcpSocket *client) {
    Q_ASSERT(client != nullptr);
    if (client->bytesAvailable() >= 28) {
        QByteArray data = client->read(28);
        if (data.length() != 28) {
            return;
        }

        qint32 id;
        cv::Point3f position;
        cv::Point3f rotation;
        QDataStream stream(&data, QIODevice::ReadOnly);
        stream.setFloatingPointPrecision(QDataStream::SinglePrecision);
        stream >> id;
        stream >> position.x;
        stream >> position.y;
        stream >> position.z;
        stream >> rotation.x;
        stream >> rotation.y;
        stream >> rotation.z;

        // We do not use the ID in the message, but assign the ID based on
        // the client's port number.
        id = client->peerPort();

        emit arViewUpdated(id, position, rotation);
    }
}

void ServerSocket::newConnection() {
    QTcpSocket *client = sock->nextPendingConnection();
    clients.append(client);
    connect(client, SIGNAL(error(QAbstractSocket::SocketError)), this, SLOT(handleClientError()));
    emit clientConnected(client);
}

void ServerSocket::handleError() {
    qDebug() << "Error in ServerSocket" << sock->errorString();
    emit errorOccurred(
                "The server has experienced an internal error.\n"
                "\n"
                "Please restart the server.");
}

void ServerSocket::handleClientError() {
    QTcpSocket *client = qobject_cast<QTcpSocket*>(sender());
    if (client != nullptr) {
        disconnect(client);
    }
}

}
