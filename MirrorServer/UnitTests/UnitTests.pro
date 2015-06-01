#-------------------------------------------------
#
# Unit Tests for the Mirror Server application.
#
#-------------------------------------------------

#-------------------------------------------------
#
# This project uses Google Test to provide the
# testing framework. The Google Test source is
# included in the project in the `gtest` folder
# and is lecensed under the new BSD license.
#
#-------------------------------------------------

QT       += core gui network

TARGET    = UnitTests
CONFIG   += console c++11
CONFIG   -= app_bundle

TEMPLATE = app

HEADERS += \
    gtest/gtest.h

SOURCES += \
    averagertest.cpp \
    utilstest.cpp \
    gtest/gtest_main.cc \
    gtest/gtest-all.cc

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

# ------ Add the dependency for the Server ------
win32:debug:        LIBS += -L$$OUT_PWD/../Server/release/ -lServer
else:win32:release: LIBS += -L$$OUT_PWD/../Server/debug/   -lServer
else:unix:          LIBS += -L$$OUT_PWD/../Server/         -lServer
INCLUDEPATH += $$PWD/../Server
DEPENDPATH  += $$PWD/../Server