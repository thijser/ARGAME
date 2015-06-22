# -----------------------------------------------------
# This subproject contains a simple HTTP server.
#
# The sources in this subproject are written by
# Nikhil Marathe, and are redistributed under the
# terms of the MIT license.
#
# See https://github.com/nikhilm/qhttpserver for the
# QHttpServer project on Github.
# -----------------------------------------------------
# A minor modification has been made to qhttpserverapi.h,
# to allow building as a static library on Windows.
# -----------------------------------------------------

TEMPLATE = lib

QT += network
QT -= gui

CONFIG += c++11 staticlib

DEFINES += QHTTPSERVER_STATIC

PRIVATE_HEADERS += http_parser.h qhttpconnection.h
PUBLIC_HEADERS += qhttpserver.h qhttprequest.h qhttpresponse.h qhttpserverapi.h qhttpserverfwd.h

HEADERS = $$PRIVATE_HEADERS $$PUBLIC_HEADERS
SOURCES = *.cpp http_parser.c
