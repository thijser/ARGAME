#-------------------------------------------------
#
# Project created by QtCreator 2015-06-01T16:13:57
#
#-------------------------------------------------

QT       += core gui network
greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = MirrorServer
TEMPLATE = app
CONFIG -= app_bundle
CONFIG += c++11

win32 {
    !win32_debug: OUT_PWD = $$OUT_PWD/../build
}

SOURCES += \
    main.cpp

# ----- Add dependency for Server project -----
win32_debug: LIBS += -L$$OUT_PWD/../Server/debug   -lServer
else:win32:  LIBS += -L$$OUT_PWD/../Server/release -lServer
else:        LIBS += -L$$OUT_PWD/../Server/        -lServer
INCLUDEPATH += $$PWD/../Server
DEPENDPATH  += $$PWD/../Server

# ---------- Add the dependency for OpenCV ----------
OPENCV_PATH = $$(OPENCV_HOME)
isEmpty(OPENCV_PATH) {
    error(OPENCV_HOME is not defined. Set OPENCV_HOME to point to the OpenCV home directory)
}
win32_debug: OPENCV_SUFFIX = 300d
else:win32: OPENCV_SUFFIX = 300

LIBS += -L$$OPENCV_PATH/lib \
        -lopencv_core$$OPENCV_SUFFIX \
        -lopencv_imgproc$$OPENCV_SUFFIX \
        -lopencv_highgui$$OPENCV_SUFFIX
win32: LIBS += -lopencv_videoio$$OPENCV_SUFFIX \
               -lopencv_video$$OPENCV_SUFFIX \
               -lopencv_imgcodecs$$OPENCV_SUFFIX
INCLUDEPATH += $$OPENCV_PATH/include
DEPENDPATH  += $$OPENCV_PATH/include
