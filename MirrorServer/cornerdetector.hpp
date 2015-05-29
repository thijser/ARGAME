/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file cornerdetector.hpp
* @brief Contains implementation of Mirrors::CornerDetector.
*/

#ifndef CORNER_DETECTOR_HPP
#define CORNER_DETECTOR_HPP

#include <opencv2/core/core.hpp>

namespace Mirrors {

using cv::Mat;
using cv::Point;
using cv::Size;
using std::vector;

/**
 * @brief Detector of four red corner markers that represent a rectangular surface.
 */
class CornerDetector {
public:
    /**
     * @brief Constructs a detector for corners in the specified image.
     * @param image - Image to detect corners in.
     */
    CornerDetector(const Mat& image);

    /**
     * @brief Retrieves the detected four corners or an empty list if a different amount was found.
     * @return List of corners in order top-left, top-right, bottom-left, bottom-right or an empty
     * list if an amount of corners other than four was found.
     */
    const vector<Point>& getCorners();

private:
    /// List of detected corners.
    vector<Point> corners;

    /**
     * @brief Finds corners in the specified image.
     * @param image - Image to find corners in.
     * @return List of corners in order top-left, top-right, bottom-left, bottom-right or an empty
     * list if an amount of corners other than four was found.
     */
    static vector<Point> findCorners(const Mat& image);

    /**
     * @brief Takes four arbitrary corner contours and classifies them as top-left, top-right and so on.
     * @param contours - Detected contours that represent corner markers in an image.
     * @return List of corners in order top-left, top-right, bottom-left and bottom-right.
     */
    static vector<Point> classifyCorners(const vector<vector<Point>>& contours);
};

}

#endif