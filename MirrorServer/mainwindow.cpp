#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "serversocket.h"

#include <QDebug>
#include <QImage>
namespace mirrors {

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow) {
    ui->setupUi(this);

    connect(ui->startButton, SIGNAL(clicked(bool)),
            this,            SLOT(startServer()));
    connect(ui->stopButton,  SIGNAL(clicked(bool)),
            this,            SLOT(stopServer()));

    // The server is stopped by default, so the stop button
    // needs to be disabled here.
    ui->stopButton->setEnabled(false);
}

MainWindow::~MainWindow() {
    delete ui;
}

void MainWindow::setServer(ServerSocket *server) {
    Q_ASSERT(server != NULL);
    this->server = server;
}

void MainWindow::setDetector(detector *det) {
    Q_ASSERT(det != NULL);
    this->det = det;
}

void MainWindow::startServer() {
    qDebug() << "Starting server";
    bool ok = true;
    int port = ui->serverPort->text().toInt(&ok);
    if (!ok || port <= 0 || port >= 65536) {
        //TODO Show message dialog instead.
        qDebug() << "Provided an invalid port number";
    } else {
        server->setPortNumber(port);
        server->start();
        ui->serverPort->setEnabled(false);
        ui->startButton->setEnabled(false);
        ui->stopButton->setEnabled(true);
        qDebug() << "Server started";
    }
}

void MainWindow::handleFrame(const cv::Mat &matrix, vector<detected_marker> markers) {
    QImage image(matrix.data, matrix.cols, matrix.rows, QImage::Format_RGB888);
    QPixmap pixmap = QPixmap::fromImage(image);
    ui->image->setPixmap(pixmap);

    for (detected_marker marker : markers) {
        if (marker.deleted) {
            server->broadcastDelete(marker.id);
        } else {
            server->broadcastPositionUpdate(marker.id, marker.position.x, marker.position.y, marker.rotation);
        }
    }
}

void MainWindow::stopServer() {
    qDebug() << "Stopping server";
    server->stop();
    ui->serverPort->setEnabled(true);
    ui->startButton->setEnabled(true);
    ui->stopButton->setEnabled(false);
    qDebug() << "Server stopped";
}
}
