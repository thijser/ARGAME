/*
 * Copyright 2015, Delft University of Technology
 *
 * This software is licensed under the terms of the MIT license.
 * See http://opensource.org/licenses/MIT for the full license.
 *
 *
 */

#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include "detector.hpp"

int main(int, char**) {
    cv::namedWindow("Camera", CV_WINDOW_AUTOSIZE);

    detector cameraDetector;

    cameraDetector.setSurfaceCorners({Point2f(423, 1), Point2f(944, 1), Point2f(344, 716), Point2f(1041, 719)});

    cameraDetector.loop([](const Mat& processedFrame, size_t markersFound) {
        Mat finalFrame(processedFrame);
        std::string debugText = "markers: " + std::to_string(markersFound);
        cv::putText(finalFrame, debugText, Point(10, 30), cv::FONT_HERSHEY_TRIPLEX, 1, cv::Scalar(0, 255, 255), 1);

        cv::imshow("Camera", finalFrame);
    });

    return 0;
}
