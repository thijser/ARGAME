#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <opencv2/core/core.hpp>
#include <vector>

namespace mirrors {
using namespace std;

class ServerController;

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow {
    Q_OBJECT
public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();

    void setController(ServerController *controller);
public slots:
    void startServer();

    void handleFrame(const cv::Mat& matrix);
    void stopServer();
private:
    Ui::MainWindow *ui;
    ServerController *controller;
};

}

#endif // MAINWINDOW_H
