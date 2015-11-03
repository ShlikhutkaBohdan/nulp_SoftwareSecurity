#-------------------------------------------------
#
# Project created by QtCreator 2015-11-03T15:55:18
#
#-------------------------------------------------

QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

win32:LIBS += -LC:/Windows/System32 -lkernel32 -lcrypt32 -lAdvapi32

TARGET = lab4_cryptoApiRsa
TEMPLATE = app


SOURCES += main.cpp\
        mainwindow.cpp \
    mycryptoapi.cpp

HEADERS  += mainwindow.h \
    mycryptoapi.h

FORMS    += mainwindow.ui
