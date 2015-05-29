#include "mainwindow.h"
#include "servercontroller.h"

#include <QApplication>
#include <QDebug>

using namespace mirrors;

/**
 * @brief Entry point of the application.
 * @param argc - The amount of command-line arguments.
 * @param argv - The command-line argument values.
 * @return The exit code of the application.
 */
int main(int argc, char *argv[]) {
    QApplication a(argc, argv);

    ServerController controller;
    MainWindow w;
    w.setController(&controller);
    w.show();

    return a.exec();
}
