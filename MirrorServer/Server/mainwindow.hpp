#ifndef MAINWINDOW_H
#define MAINWINDOW_H

/**
 * @file mainwindow.h
 * @brief Defines the mirrors::MainWindow class.
 */

#include <QMainWindow>
#include <QTcpSocket>
#include <vector>

class QErrorMessage;

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
     * @brief Display the framerate in this window.
     * @param fps - The framerate to display
     */
    void showFPS(int fps);

    /**
     * @brief Update the list of connected clients.
     * @param clients - Clients to list.
     */
    void showClients(QList<QTcpSocket*> clients);

    /**
     * @brief Set if the debug overlay should be shown.
     */
    void setDebugOverlay(bool enable);

    /**
     * @brief Starts the server.
     */
    void startServer();

    /**
     * @brief Displays the given image in this window.
     * @param image - The image to display
     */
    void handleFrame(const QPixmap& image);

    /**
     * @brief Stops the server.
     */
    void stopServer();

    /**
     * @brief Force the level to be changed to the user
     * specified level.
     */
    void forceChangeLevel();

    /**
     * @brief Compare the entered level with the current
     * level and reflect this in the change button.
     */
    void checkLevelEntry();

    /**
     * @brief Updates the level index shown on the UI.
     * @param level - The new level index.
     */
    void updateLevel(int level);

    /**
     * @brief Updates the level time in the UI.
     */
    void updateLevelTime();
private:
    /**
     * @brief Enables or disables the server configuration options.
     * @param enabled - True to enable, false to disable.
     */
    void setConfigEnabled(bool enabled);

    /// The satellite object containing the UI elements.
    Ui::MainWindow *ui;

    /// The ServerController instance used to control the server.
    ServerController *controller;

    /// The Error message dialog used to queue and show error messages.
    QErrorMessage *errorDialog;

    /// Timer to update current level time.
    QTimer *updateLevelTimer;
};

}

#endif // MAINWINDOW_H
