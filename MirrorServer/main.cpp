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

#define SERVER_PORT 23369

/**
 * @brief Entry point of this application.
 * @param argc - The amount of command-line arguments.
 * @param argv - The command-line argument values.
 * @return The exit code of this application.
 */
int main(int argc, char **argv) {
    int deviceID = 0;
    bool showFrames = true;
    if (argc > 1) {
        deviceID = atoi(argv[1]);
        std::clog << "Using camera device #" << deviceID << std::endl;
    }
    if (argc > 2) {
        showFrames = (bool) atoi(argv[2]);
        std::clog << "UI Enabled: " << showFrames << std::endl;
    }

    detector cameraDetector(deviceID);

    ServerSocket::initialize();
    ServerSocket server(SERVER_PORT);
    std::thread serverThread([&](){
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
    cameraDetector.loop([&](const Mat& processedFrame, vector<Point> markerPositions) {
        long time = duration_cast<milliseconds>(high_resolution_clock::now().time_since_epoch()).count();
        for (size_t i = 0; i < markerPositions.size(); i++) {
            server.broadcastPositionUpdate(
                        static_cast<uint32_t>(i),
                        markerPositions[i].x,
                        markerPositions[i].y,
                        time);
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
