#-------------------------------------------------
#
# Project created by QtCreator 2015-05-27T13:14:43
#
#-------------------------------------------------

QT       += core gui network
greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = Server
TEMPLATE = lib
CONFIG += c++11 precompile_header staticlib

SOURCES += \
    mainwindow.cpp \
    serversocket.cpp \
    servercontroller.cpp \
    boarddetector.cpp \
    cvutils.cpp \
    markerdetector.cpp \
    markerrecognizer.cpp \
    markertracker.cpp

HEADERS  += \
    serversocket.hpp \
    averager.hpp \
    boarddetector.hpp \
    cvutils.hpp \
    markerdetector.hpp \
    markerrecognizer.hpp \
    markertracker.hpp \
    servercontroller.hpp \
    mainwindow.hpp

PRECOMPILED_HEADER = mirrors.h

FORMS    += mainwindow.ui

# ---------- Add the dependency for OpenCV ----------
OPENCV_PATH = $$(OPENCV_HOME)
isEmpty(OPENCV_PATH) {
    error(OPENCV_HOME is not defined. Set OPENCV_HOME to point to the OpenCV home directory)
}
win32_debug: OPENCV_SUFFIX = 300d
else:win32:  OPENCV_SUFFIX = 300

LIBS += -L$$OPENCV_PATH/lib \
        -lopencv_core$$OPENCV_SUFFIX \
        -lopencv_imgproc$$OPENCV_SUFFIX \
        -lopencv_highgui$$OPENCV_SUFFIX
win32: LIBS += -lopencv_videoio$$OPENCV_SUFFIX \
               -lopencv_video$$OPENCV_SUFFIX \
               -lopencv_imgcodecs$$OPENCV_SUFFIX
INCLUDEPATH += $$OPENCV_PATH/include
DEPENDPATH += $$OPENCV_PATH/include
