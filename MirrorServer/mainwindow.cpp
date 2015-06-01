#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "servercontroller.h"

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

    // Set the camera device ID to only accept -1 and higher
    // values (-1 selects the default camera on the system).
    QIntValidator *camValidator = new QIntValidator(this);
    camValidator->setBottom(-1);
    ui->cameraDevice->setValidator(camValidator);
}

MainWindow::~MainWindow() {
    delete ui;
}

void MainWindow::setController(ServerController *controller) {
    this->controller = controller;
}

void MainWindow::startServer() {
    connect(controller, SIGNAL(imageReady(cv::Mat)),
            this,       SLOT(handleFrame(cv::Mat)));

    // The QIntValidator set to this QLineEdit
    // ensures the text represents a valid number
    // in the range 0-65536
    quint16 port = static_cast<quint16>(ui->serverPort->text().toInt());
    int device = ui->cameraDevice->text().toInt();
    controller->startServer(port, device);

    // Disable the configuration options.
    ui->serverPort->setEnabled(false);
    ui->cameraDevice->setEnabled(false);
    ui->startButton->setEnabled(false);
    ui->stopButton->setEnabled(true);
}

void MainWindow::handleFrame(const cv::Mat &matrix) {
    // OpenCV stores pixels in BGR order, but QImage expects RGB order.
    // We make a copy that contains the correct order first.
    try {
        cv::Mat copy;
        cv::cvtColor(matrix, copy, CV_BGR2RGB);
        QImage image(copy.data, copy.cols, copy.rows, copy.step, QImage::Format_RGB888);
        // Make sure the image fits on screen by scaling it to 500 px high.
        QPixmap pixmap = QPixmap::fromImage(image, Qt::ColorOnly).scaledToHeight(500);

        ui->image->setPixmap(pixmap);
        QRect rect = ui->image->geometry();
        setFixedWidth(pixmap.width() + width() - rect.width());
    } catch (const cv::Exception& ex) {
        qDebug() << "Unexpected OpenCV Exception in handleFrame: " << ex.what();
    }
}

void MainWindow::stopServer() {
    disconnect(controller, SIGNAL(imageReady(cv::Mat)),
               this,       SLOT(handleFrame(cv::Mat)));
    controller->stopServer();

    // Enable the configuration options.
    ui->serverPort->setEnabled(true);
    ui->cameraDevice->setEnabled(true);
    ui->startButton->setEnabled(true);
    ui->stopButton->setEnabled(false);
    ui->image->setPixmap(QPixmap());
}

}
