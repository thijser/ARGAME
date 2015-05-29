#ifndef MIRRORS_MAINWINDOWB_H
#define MIRRORS_MAINWINDOWB_H

#include <QMainWindow>

namespace mirrors {

namespace Ui {
class MainWindowB;
}

class MainWindowB : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindowB(QWidget *parent = 0);
    ~MainWindowB();

private:
    Ui::MainWindowB *ui;
};


} // namespace mirrors
#endif // MIRRORS_MAINWINDOWB_H
