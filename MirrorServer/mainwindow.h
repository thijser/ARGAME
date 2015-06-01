#ifndef MAINWINDOW_H
#define MAINWINDOW_H

/**
 * @file mainwindow.h
 * @brief Defines the mirrors::MainWindow class.
 */

#include <QMainWindow>
#include <vector>

namespace mirrors {
using namespace std;

class ServerController;

namespace Ui {
class MainWindow;
}

/**
 * @brief Main window for the server application.
 *
 * This class provides a UI enabling users to configure
 * the server and start / stop it.
 */
class MainWindow : public QMainWindow {
    Q_OBJECT
public:
    /**
     * @brief Creates a new MainWindow instance with the given parent.
     * @param parent - The parent QObject, may be null
     */
    explicit MainWindow(QWidget *parent = 0);
    /**
     * @brief Destroys this MainWindow instance and any resources associated with it.
     */
    ~MainWindow();

    /**
     * @brief Sets the ServerController instance this MainWindow will use.
     * @param controller - The ServerController instance.
     */
    void setController(ServerController *controller);
public slots:

    /**
     * @brief Starts the server.
     */
    void startServer();

    /**
     * @brief Displays the given CV Matrix image in this window.
     * @param matrix - The matrix image to display
     */
    void handleFrame(const cv::Mat& matrix);

    /**
     * @brief Stops the server.
     */
    void stopServer();
private:
    /// The satellite object containing the UI elements.
    Ui::MainWindow *ui;

    /// The ServerController instance used to control the server.
    ServerController *controller;
};

}

#endif // MAINWINDOW_H
