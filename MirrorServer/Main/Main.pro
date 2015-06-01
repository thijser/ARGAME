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


SOURCES += \
    main.cpp

# ----- Add dependency for Server project -----
win32:CONFIG(release, debug|release): LIBS += -L$$OUT_PWD/../Server/release/ -lServer
else:win32:CONFIG(debug, debug|release): LIBS += -L$$OUT_PWD/../Server/debug/ -lServer
else:unix: LIBS += -L$$OUT_PWD/../Server/ -lServer
INCLUDEPATH += $$PWD/../Server
DEPENDPATH  += $$PWD/../Server

# ---------- Add the dependency for OpenCV ----------
OPENCV_PATH = $$(OPENCV_HOME)
isEmpty(OPENCV_PATH) {
    error(OPENCV_HOME is not defined. Set OPENCV_HOME to point to the OpenCV home directory)
}
win32: OPENCV_SUFFIX = 300
LIBS += -L$$OPENCV_PATH/lib \
        -lopencv_core$$OPENCV_SUFFIX \
        -lopencv_imgproc$$OPENCV_SUFFIX \
        -lopencv_highgui$$OPENCV_SUFFIX
INCLUDEPATH += $$OPENCV_PATH/include
DEPENDPATH += $$OPENCV_PATH/include
