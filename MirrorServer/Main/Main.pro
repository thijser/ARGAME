#-------------------------------------------------
#
# Project created by QtCreator 2015-06-01T16:13:57
#
#-------------------------------------------------
lessThan(QT_MAJOR_VERSION, 5): error("This project requires at least Qt 5")
QT       += core gui network widgets

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

# ----- Add dependency for the QHttpServer project -----
win32_debug: LIBS += -L$$OUT_PWD/../QHttpServer/debug   -lQHttpServer
else:win32:  LIBS += -L$$OUT_PWD/../QHttpServer/release -lQHttpServer
else:        LIBS += -L$$OUT_PWD/../QHttpServer/        -lQHttpServer
INCLUDEPATH += $$PWD/../QHttpServer
DEPENDPATH  += $$PWD/../QHttpServer
# We link to QHttpServer statically
DEFINES += QHTTPSERVER_STATIC

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

# ------------- Deployment Options ------------
# Output to 'dist' directory in the repository root.
DESTDIR = $$PWD/../../dist

win32 {
    win32_debug: DEPLOY_MODE = debug
    else:        DEPLOY_MODE = release
    WIN_DESTDIR = $$replace(DESTDIR, /, \\)
    # The 'deployWindows' batch file collects all dependant libraries and places them in $$DESTDIR
    QMAKE_POST_LINK += $$PWD/../deployWindows.bat $$DEPLOY_MODE "$$(OPENCV_HOME)" "$$(QTDIR)" $$WIN_DESTDIR
}
