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

    cameraDetector.setSurfaceCorners({Point2f(171, 2), Point2f(518, 2), Point2f(121, 477), Point2f(586, 475)});

    cameraDetector.loop([](const Mat& processedFrame, size_t markersFound) {
        std::string debugText = "markers: " + std::to_string(markersFound);
        cv::putText(processedFrame, debugText, Point(10, 30), cv::FONT_HERSHEY_TRIPLEX, 1, cv::Scalar(0, 255, 255), 1);

        cv::imshow("Camera", processedFrame);

        cv::waitKey(10);
    });

    return 0;
}
