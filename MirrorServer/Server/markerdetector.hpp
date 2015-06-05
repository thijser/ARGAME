/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file markerdetector.hpp
* @brief Contains implementation of mirrors::MarkerDetector.
*/

#ifndef MARKER_DETECTOR_HPP
#define MARKER_DETECTOR_HPP

namespace mirrors {

    using cv::Mat;
    using cv::Point;
    using std::vector;

    namespace MarkerDetectionApproach {
        /**
        * @brief Method of detecting the bounds of markers.
        */
        enum MarkerDetectionApproach {
            GREEN_BORDERS
        };
    }

    /**
     * @brief Finds the contours of markers in a board image.
     */
    class MarkerDetector {
    public:
        /**
         * @brief Constructs a marker contour detector using the specified technique.
         * @param approach - Technique for locating markers on the board.
         */
        explicit MarkerDetector(MarkerDetectionApproach::MarkerDetectionApproach approach = MarkerDetectionApproach::GREEN_BORDERS)
            : approach(approach) {}

        /**
         * @brief Locates markers in the board image and returns their outer contours.
         * @param boardImage - Board image to detect markers in.
         * @return Contours of marker patterns on board.
         */
        vector<vector<Point>> locateMarkers(const Mat& boardImage) const;

    private:
        /// Technique for finding markers.
        MarkerDetectionApproach::MarkerDetectionApproach approach;

        /**
         * @brief Create a mask with the large green regions in the specified image.
         * @param image - Image to find green regions in.
         * @return Mask of green regions in specified image.
         */
        static Mat thresholdGreen(const Mat& image);

        /**
         * @brief Finds the contours that look like markers from a mask.
         * @param mask - Mask with marker regions created from board image.
         * @return List of outer contours of likely markers.
         */
        static vector<vector<Point>> findMarkerContours(const Mat& mask);
    };

}

#endif
