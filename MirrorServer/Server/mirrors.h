
/**
 * @file mirrors.h
 * @brief Precompiled header file.
 *
 * This file contains project-wide documentation elements and
 * acts as a precompiled header for the Mirrors server.
 */

/**
 * @namespace mirrors
 * @brief Root namespace of the Mirrors Server.
 */

/**
 * @namespace mirrors::bytes
 * @brief Provides byte order conversion functions.
 *
 * The functions in this utility namespace can be used to safely transmit
 * data over the network using a known byte order (big-endian). It also
 * performs conversion from value types to QByteArrays and vice versa.
 */

/**
 * @mainpage
 *
 * This is a server application for providing marker detection to the Mirrors AR game.
 *
 * This server provides relative positions and rotations of markers to the
 * Unity-based client applications. The application contains three main components:
 *
 * @li The detector class, which provides marker detection functionality
 * @li The ServerSocket class, which maintains socket connections to clients.
 * @li The MainWindow class, which provides a user interface for managing the server.
 *
 * The application depends on Qt for the UI and Socket connectivity, as well as the
 * signal/slot mechanism provided by Qt. Because Qt is used, the server application
 * can be compiled for different platforms with minimal effort. The application is
 * designed for Qt 5.4, although using earlier versions of Qt may also work.
 */

#ifdef __cplusplus
// Include standard headers that are used throughout the project.
#include <QObject>
#include <QDebug>

#include <opencv2/core/core.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>

#endif
