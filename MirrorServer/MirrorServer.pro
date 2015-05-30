#-------------------------------------------------
#
# Project created by QtCreator 2015-05-27T13:14:43
#
#-------------------------------------------------

QT       += core gui network
greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = MirrorServer
TEMPLATE = app
CONFIG += c++11 precompile_header

SOURCES += \
    main.cpp \
    mainwindow.cpp \
    serversocket.cpp \
    detector.cpp \
    servercontroller.cpp

HEADERS  += \
    mainwindow.h \
    serversocket.h \
    ringbuffer.hpp \
    detector.hpp \
    servercontroller.h

PRECOMPILED_HEADER = mirrors.h

FORMS    += mainwindow.ui

# ---------- Add the dependency for OpenCV ----------
# This depends on the environment variable named
# OPENCV_HOME to point to the OpenCV home directory.
# (set this under "Projects > Build Environment" in
# Qt Creator).
OPENCV_PATH = $$(OPENCV_HOME)
isEmpty(OPENCV_PATH) {
    error(OPENCV_HOME is not defined. Set OPENCV_HOME to point to the OpenCV home directory)
}
LIBS += -L$$OPENCV_PATH/lib -lopencv_core -lopencv_imgproc -lopencv_highgui
INCLUDEPATH += $$OPENCV_PATH/include
DEPENDPATH += $$OPENCV_PATH/include
