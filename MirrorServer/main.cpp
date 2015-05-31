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

#include "markertracker.hpp"

using namespace mirrors;

int main(int argc, char** argv) {
    // Set up detectors and recognizers using default techniques
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;

    for (int i = 0; i < 12; i++) {
        auto pattern = cv::imread("markers/" + std::to_string(i) + ".png", CV_LOAD_IMAGE_GRAYSCALE);
        markerRecognizer.registerPattern(i, pattern);
    }

    // Set up marker tracker
    MarkerTracker markerTracker(boardDetector, markerDetector, markerRecognizer);

    Mat fakeFrame = cv::imread("input_remove.png");
    Mat fakeFrame2 = cv::imread("input_fastmove.png");
    boardDetector.locateBoard(fakeFrame);

    auto updates = markerTracker.track(fakeFrame, 0);

    updates = markerTracker.track(fakeFrame2, 10);

    return EXIT_SUCCESS;
}
