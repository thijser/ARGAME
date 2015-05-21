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
using namespace std;

#include <opencv2/highgui/highgui.hpp>

#include "detector.hpp"
#include "serversocket.h"
using namespace mirrors;

#define SERVER_PORT 23369

int main(int, char**) {
    detector cameraDetector;
    cv::namedWindow("Camera", CV_WINDOW_AUTOSIZE);

    ServerSocket::initialize();
    ServerSocket server(SERVER_PORT);
    std::thread serverThread([&](){
        try {
            server.run();
        } catch (const NL::Exception &ex) {
            cerr << "Uncaught exception in server: " << ex.what() << endl;
            cameraDetector.stop();
        }
    });

    cameraDetector.setSurfaceCorners({Point2f(423, 1), Point2f(944, 1), Point2f(344, 716), Point2f(1041, 719)});

    cameraDetector.loop([](const Mat& processedFrame) {
        cv::imshow("Camera", processedFrame);
    });

    // Disconnect the Socket and wait for the server to stop.
    server.disconnect();
    serverThread.join();

    return EXIT_SUCCESS;
}
