#include "mainwindowb.h"
#include "ui_mainwindowb.h"

namespace mirrors {

MainWindowB::MainWindowB(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindowB)
{
    ui->setupUi(this);
}

MainWindowB::~MainWindowB()
{
    delete ui;
}

} // namespace mirrors
