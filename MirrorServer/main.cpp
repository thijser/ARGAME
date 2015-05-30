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

#include "boarddetector.hpp"

using namespace mirrors;

int main(int argc, char** argv) {
    auto image = cv::imread("input.png");

    BoardDetector boardDetector;
    boardDetector.locateBoard(image);
    Mat boardImage = boardDetector.extractBoard(image);

    cv::imshow("Board image", boardImage);
    cv::waitKey(0);

    return EXIT_SUCCESS;
}
