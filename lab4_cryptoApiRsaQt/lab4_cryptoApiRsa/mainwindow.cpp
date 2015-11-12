#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QMessageBox>
#include "mycryptoapi.h"
#include "encrypter_demo.h"

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
    qwe q;
   // Keys(_T("e:\\1.pub.txt"), _T("e:\\1.priv.txt"));

    Encrypt(_T("e:\\1.pub.txt"), _T("e:\\1.txt"), _T("e:\\2.txt"));
}
