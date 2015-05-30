/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file cvutils.hpp
* @brief Types and methods useful for OpenCV operations, but missing in the library.
*/

#ifndef OPENCV_UTILS_HPP
#define OPENCV_UTILS_HPP

#include <opencv2/core/core.hpp>

namespace mirrors {

    using cv::Mat;

    /**
     * @brief Order of channels in image using BGR color space. 
     */
    namespace BGR {
        enum BGR {
            B = 0,
            G,
            R
        };
    }

    /**
     * @brief Order of channels in image using HSV color space.
     */
    namespace HSV {
        enum HSV {
            H = 0,
            S,
            V
        };
    }

    /**
     * @brief Type of element in cv::findContours() hierarchy vector by index.
     */
    namespace HierarchyElement {
        enum HierarchyElement {
            NEXT = 0,
            PREVIOUS,
            CHILD,
            PARENT
        };
    }

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
    * @brief Rotate image by arbitrary angle.
    * @param src - Input image.
    * @param angle - Angle rotate image by in clockwise direction (may be negative for counter-clockwise).
    * @return Rotated image, which may be resized to fit the result.
    */
    Mat rotateImage(Mat src, float angle);

    /**
    * @brief Rotate image by multiple of 90 degrees.
    * @param src - Input image.
    * @param angle - Multiple of 90 degrees.
    * @return Rotated image, where row/column size may be swapped.
    */
    Mat rotateExactly(Mat src, ExactAngle angle);

}

#endif