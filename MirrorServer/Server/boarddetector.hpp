/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file boarddetector.hpp
* @brief Contains implementation of mirrors::BoardDetector.
*/

#ifndef BOARD_DETECTOR_HPP
#define BOARD_DETECTOR_HPP

namespace mirrors {

    using cv::Mat;
    using cv::Point;
    using std::vector;

    namespace BoardDetectionApproach {
        /**
        * @brief Method of detecting the bounds of the board surface.
        */
        enum BoardDetectionApproach {
            RED_MARKERS
        };
    }

    /**
     * @brief Finds the bounds of the board surface in a camera image
     * and isolates the image of the board.
     */
    class BoardDetector {
    public:
        /**
         * @brief Constructs a board surface detector that uses the specified technique.
         * @param approach - Technique for locating the board surface.
         */
        explicit BoardDetector(BoardDetectionApproach::BoardDetectionApproach approach = BoardDetectionApproach::RED_MARKERS)
            : approach(approach) {}

        /**
         * @brief (Re)locates the bounds of the board surface from the specified camera image.
         * @param cameraImage - Image to locate board in.
         * @return Boolean indicating if the four corners of the board were successfully found.
         */
        bool locateBoard(const Mat& cameraImage);

        /**
         * @brief Isolates the board from the input image if the most recent call to
         * locateBoard() was successful and the camera and board haven't moved.
         * @param cameraImage - Image to locate board in.
         * @return Isolated board image or empty image if no corners were found yet.
         */
        Mat extractBoard(const Mat& cameraImage) const;

    private:
        /// Corners found in most recent locateBoard() call.
        vector<Point> corners;

        /// Technique for finding board corners.
        BoardDetectionApproach::BoardDetectionApproach approach;

        /**
         * @brief Finds the red markers that represent the corners of the board.
         * @param cameraImage - Image to find red markers in.
         * @return List of contours that represent possible markers.
         */
        vector<vector<Point>> findMarkers(const Mat& cameraImage) const;

        /**
         * @brief Determines the four corners of the board from the marker contours
         * found using findMarkers().
         * @param markerContours - Corner marker contours from findMarkers().
         * @return List of four corner points in the order top-left, top-right,
         * bottom-left and bottom-right.
         */
        vector<Point> classifyMarkers(const vector<vector<Point>>& markerContours) const;
    };

}

#endif
