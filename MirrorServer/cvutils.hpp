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

namespace mirrors {

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

}

#endif