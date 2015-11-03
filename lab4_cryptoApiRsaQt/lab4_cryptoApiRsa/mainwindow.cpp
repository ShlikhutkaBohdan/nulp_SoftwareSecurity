#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QMessageBox>
#include "mycryptoapi.h"

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_pushButton_clicked()
{
    MyCryptoApi *api = new MyCryptoApi();
    api->ImportKey();
    api->Encrypt();
    api->Decrypt();
    QMessageBox::information(0, "error", "end!");
    delete api;
}
