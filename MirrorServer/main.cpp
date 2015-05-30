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

using namespace mirrors;

int main(int argc, char** argv) {
    auto boardImage = cv::imread("input.png");

    MarkerDetector markerDetector;
    auto markerContours = markerDetector.locateMarkers(boardImage);

    std::cout << markerContours.size() << std::endl;

    std::cin.get();

    return EXIT_SUCCESS;
}
