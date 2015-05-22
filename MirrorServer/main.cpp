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

#include <opencv2/highgui/highgui.hpp>

#include "detector.hpp"
#include "serversocket.h"
using namespace mirrors;

#define SERVER_PORT 23369

int main(int argc, char **argv) {
    int deviceID = 0;
    if (argc > 1) {
        deviceID = atoi(argv[1]);
        std::clog << "Using camera device #" << deviceID << std::endl;
    }

    detector cameraDetector(deviceID);
    cv::startWindowThread();
    cv::namedWindow("Camera", CV_WINDOW_AUTOSIZE);

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

    cameraDetector.loop([&server](const Mat& processedFrame, vector<Point> markerPositions) {
        static auto start_time = std::chrono::high_resolution_clock::now();

        // Determine time for updates
        auto t = (std::chrono::high_resolution_clock::now() - start_time).count();

        // Send positions of markers
        for (size_t i = 0; i < markerPositions.size(); i++) {
            server.broadcastPositionUpdate(static_cast<uint32_t>(i), markerPositions[i].x, markerPositions[i].y, t);
        }

        cv::imshow("Camera", processedFrame);
    });

    // Disconnect the Socket and wait for the server to stop.
    server.disconnect();
    serverThread.join();

    return EXIT_SUCCESS;
}
