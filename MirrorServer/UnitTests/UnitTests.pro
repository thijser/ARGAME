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
# and is licensed under the new BSD license.
#
#-------------------------------------------------

QT       += core gui network

TARGET    = UnitTests
CONFIG   += console c++11
CONFIG   -= app_bundle

TEMPLATE = app

HEADERS += \
    gtest/gtest.h \
    testutilities.hpp

SOURCES += \
    averagertest.cpp \
    utilstest.cpp \
    gtest/gtest_main.cc \
    gtest/gtest-all.cc \
    boarddetectortest.cpp \
    testutilities.cpp \
    markerdetectortest.cpp \
    markerrecognizertest.cpp \
    markertrackertest.cpp

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

# ------ Add the dependency for the Server ------
win32_debug: LIBS += -L$$OUT_PWD/../Server/debug   -lServer
else:win32:  LIBS += -L$$OUT_PWD/../Server/release -lServer
else:        LIBS += -L$$OUT_PWD/../Server/        -lServer
INCLUDEPATH += $$PWD/../Server
DEPENDPATH  += $$PWD/../Server
