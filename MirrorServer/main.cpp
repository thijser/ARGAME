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

#include "markerdetector.hpp"
#include "markerrecognizer.hpp"

using namespace mirrors;

int main(int argc, char** argv) {
    auto boardImage = cv::imread("input.png");

    MarkerDetector markerDetector;
    auto markerContours = markerDetector.locateMarkers(boardImage);

    MarkerRecognizer markerRecognizer;

    for (int i = 0; i < 12; i++) {
        auto pattern = cv::imread("markers/" + std::to_string(i) + ".png", CV_LOAD_IMAGE_GRAYSCALE);
        markerRecognizer.registerPattern(i, pattern);
    }

    auto firstMarker = markerRecognizer.recognizeMarker(boardImage, markerContours[0]);

    std::cout << firstMarker.confidence << ", " << firstMarker.id << ", " << firstMarker.rotation << std::endl;

    return EXIT_SUCCESS;
}
