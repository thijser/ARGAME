#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <opencv2/core/core.hpp>
#include <vector>

#include "detector.hpp"

namespace mirrors {
using namespace std;

class ServerSocket;

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow {
    Q_OBJECT
public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();

    void setServer(ServerSocket *server);
    void setDetector(detector *det);
public slots:
    void startServer();
    void handleFrame(const cv::Mat& matrix, vector<detected_marker> markers);
    void stopServer();
private:
    Ui::MainWindow *ui;
    ServerSocket *server;
    detector *det;
};

}

#endif // MAINWINDOW_H
