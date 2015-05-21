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

#include <opencv2/highgui/highgui.hpp>

#include "detector.hpp"
#include "serversocket.h"
using namespace mirrors;

#define SERVER_PORT 23369

int main(int, char**) {
    cv::namedWindow("Camera", CV_WINDOW_AUTOSIZE);

    ServerSocket server(SERVER_PORT);
    std::thread serverThread([&](){
        try {
            server.run();
        } catch (const NL::Exception &ex) {
            cerr << "Uncaught exception in server: " << ex.what() << endl;
        }
    });

    detector cameraDetector;

    cameraDetector.setSurfaceCorners({Point2f(423, 1), Point2f(944, 1), Point2f(344, 716), Point2f(1041, 719)});

    cameraDetector.loop([](const Mat& processedFrame) {
        Mat finalFrame(processedFrame);

        // Add text next to each marker
        /*for (size_t i = 0; i < markers.size(); i++) {
            std::string text = std::to_string(i);
            cv::putText(finalFrame, text, markers[i], cv::FONT_HERSHEY_PLAIN, 2, cv::Scalar(0, 255, 255), 2);
        }*/

        cv::imshow("Camera", finalFrame);
    });

    // Disconnect the Socket and wait for the server to stop.
    server.disconnect();
    serverThread.join();

    return 0;
}
