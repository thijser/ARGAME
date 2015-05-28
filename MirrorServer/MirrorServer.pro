#---------------------------------------------------
#
#  Copyright 2015, Delft University of Technology
#
#  This software is licensed under the terms of the MIT license.
#  See http://opensource.org/licenses/MIT for the full license.
#
#---------------------------------------------------

# ------------- Basic Project settings -------------
TARGET   = MirrorServer
TEMPLATE = app

# Disable linking to default Qt libraries.
QT      -= core gui
CONFIG  -= qt

# We're building a simple console application,
# and we want c++11 functionality.
CONFIG  += c++11
CONFIG(debug): DEFINES += DEBUG

# NetLink also uses .inc as include file extension
# We tell QMake to treat .inc files as header files.
QMAKE_EXT_H += .inc

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

# ------- Add dependency for Windows Sockets --------
win32: LIBS += -lWS2_32

# ----------------- Project sources -----------------
HEADERS += \
    netlink/config.h \
    netlink/core.h \
    netlink/exception.h \
    netlink/netlink.h \
    netlink/release_manager.h \
    netlink/release_manager.inline.h \
    netlink/smart_buffer.h \
    netlink/smart_buffer.inline.h \
    netlink/socket_group.h \
    netlink/socket_group.inline.h \
    netlink/socket.h \
    netlink/socket.inline.h \
    netlink/util.h \
    netlink/util.inline.h \
    netlink/exception.code.inc \
    serversocket.h \
    detector.hpp \
    mirrors.h

SOURCES += \
    main.cpp \
    netlink/core.cc \
    netlink/smart_buffer.cc \
    netlink/socket_group.cc \
    netlink/socket.cc \
    netlink/util.cc \
    serversocket.cpp \
    detector.cpp
