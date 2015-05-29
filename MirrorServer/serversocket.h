#ifndef SERVERSOCKET_H
#define SERVERSOCKET_H

#include <QObject>
#include <QTimer>

class QTcpServer;
class QTcpSocket;

namespace mirrors {

class ServerSocket : public QObject
{
    Q_OBJECT
    Q_DISABLE_COPY(ServerSocket)
private:
    QTcpServer *sock;
    QList<QTcpSocket*> clients;
    QTimer *pingTimer;

    int port;
public:
    explicit ServerSocket(QObject *parent = 0);

    int portNumber() const { return port; }
    QTcpServer* socket() const { return sock; }
    QList<QTcpSocket*> connections() const { return clients; }

    void setPortNumber(int portNum);
signals:
    void started();
    void stopped();

    void clientConnected(QTcpSocket *client);
    void clientDisconnected(QTcpSocket *client);

    void errorOccurred(QString message);

public slots:
    void start();
    void stop();

    void disconnect(QTcpSocket *client);

    void broadcastBytes(QByteArray bytes);
    void broadcastPositionUpdate(int id, float x, float y, float rotation);
    void broadcastDelete(int id);
    void broadcastPing();

private slots:
    void newConnection();
    void handleError();
    void handleClientError();
};

}

#endif // SERVERSOCKET_H
