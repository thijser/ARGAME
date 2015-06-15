#include "mainwindow.hpp"
#include "ui_mainwindow.h"
#include "servercontroller.hpp"

#include <QImage>
#include <QDialog>
#include <QErrorMessage>

namespace mirrors {

MainWindow::MainWindow(QWidget *parent) :
        QMainWindow(parent),
        ui(new Ui::MainWindow),
        errorDialog(QErrorMessage::qtHandler()) {
    ui->setupUi(this);

    connect(ui->startButton, SIGNAL(clicked(bool)),
            this,            SLOT(startServer()));
    connect(ui->stopButton,  SIGNAL(clicked(bool)),
            this,            SLOT(stopServer()));
    connect(ui->debugCheck,  SIGNAL(clicked(bool)),
            this,            SLOT(setDebugOverlay(bool)));

    // Set the port number LineEdit to only accept numbers in
    // the range 0-65536
    ui->serverPort->setValidator(new QIntValidator(0, 65536, this));

    // Set the camera device ID to only accept -1 and higher
    // values (-1 selects the default camera on the system).
    QIntValidator *camValidator = new QIntValidator(this);
    camValidator->setBottom(-1);
    ui->cameraDevice->setValidator(camValidator);

    // Set the width and height to only accept positive values.
    QIntValidator *sizeValidator = new QIntValidator(this);
    sizeValidator->setBottom(1);
    ui->camHeight->setValidator(sizeValidator);
    ui->camWidth->setValidator(sizeValidator);

    // Enable configuration and set proper constraints for
    // a disabled server state.
    setConfigEnabled(true);
}

MainWindow::~MainWindow() {
    delete ui;
}

void MainWindow::setController(ServerController *controller) {
    this->controller = controller;
}

void MainWindow::setDebugOverlay(bool enable) {
    this->controller->setDebugOverlay(enable);
}

void MainWindow::startServer() {
    connect(controller,  SIGNAL(imageReady(cv::Mat)),
            this,        SLOT(handleFrame(cv::Mat)));
    connect(controller,  SIGNAL(fatalErrorOccurred(QString)),
            errorDialog, SLOT(showMessage(QString)));
    connect(controller,  SIGNAL(fpsChanged(int)),
            this, SLOT(showFPS(int)));

    // Disable the configuration options.
    setConfigEnabled(false);

    // The QIntValidator set to this QLineEdit
    // ensures the text represents a valid number
    // in the range 0-65536
    quint16 port = static_cast<quint16>(ui->serverPort->text().toInt());
    int device = ui->cameraDevice->text().toInt();
    cv::Size requestedSize(
        ui->camWidth->text().toInt(),
        ui->camHeight->text().toInt()
    );

    // Determine board detection approach based on user choice
    BoardDetectionApproach::Type boardDetectionApproach;

    if (ui->cornerChoiceRed->isChecked()) {
        boardDetectionApproach = BoardDetectionApproach::RED_MARKERS;
    } else {
        boardDetectionApproach = BoardDetectionApproach::RED_YELLOW_MARKERS;
    }

    controller->startServer(port, device, requestedSize, boardDetectionApproach);

    // controller now has the actual camera resolution.
    // We update the UI to show the resolution being used.
    cv::Size size = controller->resolution();
    ui->camWidth->setText(QString::number(size.width));
    ui->camHeight->setText(QString::number(size.height));
}

void MainWindow::handleFrame(const cv::Mat &matrix) {
    // OpenCV stores pixels in BGR order, but QImage expects RGB order.
    // We make a copy that contains the correct order first.
    try {
        cv::Mat copy;
        cv::cvtColor(matrix, copy, CV_BGR2RGB);
        QImage image(copy.data, copy.cols, copy.rows, static_cast<int>(copy.step), QImage::Format_RGB888);
        // Make sure the image fits on screen by scaling it to 500 px high.
        QPixmap pixmap = QPixmap::fromImage(image, Qt::ColorOnly).scaledToHeight(500);

        ui->image->setPixmap(pixmap);
    } catch (const cv::Exception& ex) {
        qDebug() << tr("Unexpected OpenCV Exception in handleFrame:") << ex.what();
    }

}

void MainWindow::showFPS(int fps) {
    ui->fps->setText(QString::number(fps));
}

void MainWindow::stopServer() {
    disconnect(controller, SIGNAL(imageReady(cv::Mat)),
               this,       SLOT(handleFrame(cv::Mat)));
    controller->stopServer();

    // Enable the configuration options.
    setConfigEnabled(true);
}

void MainWindow::setConfigEnabled(bool enabled) {
    ui->serverPort->setEnabled(enabled);
    ui->cameraDevice->setEnabled(enabled);
    ui->startButton->setEnabled(enabled);
    ui->stopButton->setEnabled(!enabled);
    ui->camWidth->setEnabled(enabled);
    ui->camHeight->setEnabled(enabled);
    ui->cornerChoiceRed->setEnabled(enabled);
    ui->cornerChoiceRedYellow->setEnabled(enabled);

    if (enabled) {
        ui->image->setText(tr("Server stopped.\n\nClick the \"Start Server\" button."));
    } else {
        ui->image->setText(tr("Calibrating...\n\nMake sure the entire board is visible for the camera."));
    }
}

}
