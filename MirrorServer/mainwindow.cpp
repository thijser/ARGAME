#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "servercontroller.h"

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

    // Set the port number LineEdit to only accept numbers in
    // the range 0-65536
    ui->serverPort->setValidator(new QIntValidator(0, 65536, this));
}

MainWindow::~MainWindow() {
    delete ui;
}

void MainWindow::setController(ServerController *controller) {
    this->controller = controller;
}

void MainWindow::startServer() {
    qDebug() << "Starting server";

    // The QIntValidator set to this QLineEdit
    // ensures the text represents a valid number
    // in the range 0-65536
    quint16 port = static_cast<quint16>(ui->serverPort->text().toInt());
    controller->startServer(port);
    ui->serverPort->setEnabled(false);
    ui->startButton->setEnabled(false);
    ui->stopButton->setEnabled(true);
    qDebug() << "Server started";
}

void MainWindow::handleFrame(const cv::Mat &matrix) {
    QImage image(matrix.data, matrix.cols, matrix.rows, QImage::Format_RGB888);
    QPixmap pixmap = QPixmap::fromImage(image);
    ui->image->setPixmap(pixmap);
}

void MainWindow::stopServer() {
    qDebug() << "Stopping server";
    controller->stopServer();
    ui->serverPort->setEnabled(true);
    ui->startButton->setEnabled(true);
    ui->stopButton->setEnabled(false);
    qDebug() << "Server stopped";
}
}
