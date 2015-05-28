/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

#include <iostream>
#include <thread>
#include <chrono>
using std::chrono::duration_cast;
using std::chrono::high_resolution_clock;
using std::chrono::milliseconds;

#include <opencv2/highgui/highgui.hpp>

#include "detector.hpp"
#include "serversocket.h"
using namespace mirrors;

/// The port used for the server socket.
const uint16_t SERVER_PORT = 23369;

/**
 * Dummy function for handling SIGPIPE, required
 * to stop server crashes on a broken pipe (non-Windows).
 */
void handle_sigpipe(int) {
#ifdef DEBUG
    std::clog << "DEBUG: Received SIGPIPE: " << std::endl;
#endif
}

/**
* @brief Entry point of this application.
*
* The first argument to the application indicates the
* camera device ID. If omitted, 0 is used as device ID.
*
* The second argument indicates if frames are displayed
* or not. A non-zero value indicates frames should be
* displayed, while a value of 0 skips showing the frames.
* The default value is to show the frames.
*
* @param argc - The amount of command-line arguments.
* @param argv - The command-line argument values.
* @return The exit code of this application.
*/
int main(int argc, char **argv) {
#ifndef WIN32
    signal(SIGPIPE, handle_sigpipe);
#endif

    int deviceID = 0;
    bool showFrames = true;
    if (argc > 1) {
        deviceID = atoi(argv[1]);
    }
    if (argc > 2) {
        showFrames = atoi(argv[2]) != 0;
    }

#ifdef DEBUG
    std::clog << "Using camera device #" << deviceID << std::endl;
    std::clog << "UI Enabled: " << showFrames << std::endl;
#endif

    detector cameraDetector(deviceID);

    ServerSocket::initialize();
    ServerSocket server(SERVER_PORT);
    std::thread serverThread([&]() {
        try {
            server.run();
        } catch (const NL::Exception &ex) {
            std::cerr << "Uncaught exception in server: " << ex.what() << std::endl;
            cameraDetector.stop();
        }
    });

    if (showFrames) {
        cv::startWindowThread();
        cv::namedWindow("Camera", CV_WINDOW_AUTOSIZE);
    }

    std::vector<Mat> markerPatterns;

    for (int i = 0; i < 12; i++) {
        markerPatterns.push_back(cv::imread("markers/" + std::to_string(i) + ".png", CV_LOAD_IMAGE_GRAYSCALE));
    }

    bool markersLoaded = cameraDetector.registerMarkers(markerPatterns);

    if (!markersLoaded) {
        std::cerr << "Failed to load markers!" << std::endl;
        return 1;
    }
#ifdef DEBUG
    std::clog << "Detector initialized (loaded " << markerPatterns.size() << " markers)" << std::endl;
#endif

    // Start detection loop
    cameraDetector.loop([&](const Mat& processedFrame, vector<detected_marker> markers) {
        auto time = duration_cast<milliseconds>(high_resolution_clock::now().time_since_epoch()).count();

        for (auto& marker : markers) {
            if (marker.deleted) {
                server.broadcastDelete(marker.id);
            } else {
                server.broadcastPositionUpdate(marker.id, marker.position.x, marker.position.y, marker.rotation, time);
            }
        }

        if (showFrames) {
            cv::imshow("Camera", processedFrame);
        }
    });

    // Disconnect the Socket and wait for the server and detector to stop.
    server.disconnect();
    serverThread.join();

    return EXIT_SUCCESS;
}
