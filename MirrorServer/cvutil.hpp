/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file util.hpp
* @brief Contains implementation of utility functions.
*/

#ifndef CV_UTIL_HPP
#define CV_UTIL_HPP

#include <vector>
#include <opencv2/core/core.hpp>

namespace Mirrors {

using std::vector;
using cv::Point;
using cv::Mat;

/**
* @brief Angles that are multiples of 90 degrees, used for exact rotations.
*/
enum ExactAngle {
    CLOCKWISE_0 = 0,
    CLOCKWISE_90 = 90,
    CLOCKWISE_180 = 180,
    CLOCKWISE_270 = 270
};

/**
* @brief Calculate average of given numbers.
* @param vals - Collection of values to calculate average from.
* @return Average value of given numbers.
*/
float average(const vector<float>& vals);

/**
* @brief Calculate average of given points.
* @param vals - Collection of points to calculate average from.
* @return Average value of given points.
*/
Point average(const vector<Point>& vals);

/**
* @brief Calculate Euclidean distance between two points.
* @param a - First point.
* @param b - Second point.
* @return Euclidean distance between the first and second point.
*/
float dist(const Point& a, const Point& b);

/**
* @brief Calculate the center of the bounding box given the contour.
* @param contour - The contour that represents the shape of the marker.
* @return The center point of the bounding box from the contour.
*/
Point boundingCenter(const vector<Point>& contour);

/**
* @brief Rotate image by arbitrary angle.
* @param src - Input image.
* @param angle - Angle rotate image by in clockwise direction (may be negative for counter-clockwise).
* @return Rotated image, which may be resized to fit the result.
*/
Mat rotate(Mat src, float angle);

/**
* @brief Rotate image by multiple of 90 degrees.
* @param src - Input image.
* @param angle - Multiple of 90 degrees.
* @return Rotated image, where row/column size may be swapped.
*/
Mat rotateExact(Mat src, ExactAngle angle);

}

#endif