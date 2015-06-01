#-------------------------------------------------
#
# Unit Tests for the Mirror Server application.
#
#-------------------------------------------------

QT       += core
QT       -= gui

TARGET    = UnitTests
CONFIG   += console c++11
CONFIG   -= app_bundle

TEMPLATE = app

SOURCES += \
    averagertest.cpp \
    utilstest.cpp

# ----- Add dependency for Server project --------
INCLUDEPATH += ../Server
DEPENDPATH  += ../Server

# ----- Add dependency for Google test. ----------
GTEST_PATH = $$(GTEST_HOME)
isEmpty(GTEST_PATH): error(Missing GTest libraries. set GTEST_HOME in environment.)
win32:CONFIG(release, debug|release):    LIBS += -L$$GTEST_PATH -lgtest
else:win32:CONFIG(debug, debug|release): LIBS += -L$$GTEST_PATH -lgtestd
else:mac:  LIBS += -F$$GTEST_PATH -framework gtest
else:unix: LIBS += -L$$GTEST_PATH -lgtest
INCLUDEPATH += $$GTEST_PATH/include
DEPENDPATH  += $$GTEST_PATH/include

# ---------- Add the dependency for OpenCV ----------
OPENCV_PATH = $$(OPENCV_HOME)
isEmpty(OPENCV_PATH) {
    error(OPENCV_HOME is not defined. Set OPENCV_HOME to point to the OpenCV home directory)
}
LIBS += -L$$OPENCV_PATH/lib -lopencv_core -lopencv_imgproc -lopencv_highgui
INCLUDEPATH += $$OPENCV_PATH/include
DEPENDPATH += $$OPENCV_PATH/include
