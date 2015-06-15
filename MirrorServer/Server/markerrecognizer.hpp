/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file markerrecognizer.hpp
* @brief Contains implementation of mirrors::MarkerRecognizer.
*/

#ifndef MARKER_RECOGNIZER_HPP
#define MARKER_RECOGNIZER_HPP

#include <map>
#include "cvutils.hpp"

namespace mirrors {

    using cv::Mat;
    using cv::Point;
    using std::vector;
    using std::pair;

    namespace MarkerRecognitionApproach {
        /**
        * @brief Method of recognizing a marker and its pattern.
        */
        enum Type {
            HAMMING_DISTANCE
        };
    }

    class MarkerRecognizer;

    /**
     * @brief Info about a match between a detected pattern and a known pattern.
     */
    class PatternMatch {
        friend class MarkerRecognizer;

    public:
        /// ID of known marker.
        int id;

        /// Rotation of detected marker relative to known marker.
        float rotation;

        /// Scale of detected marker relative to known marker.
        float scale;

        /// Confidence of match.
        float confidence;

        /**
        * @brief Construct an empty match (no known pattern).
        */
        PatternMatch()
            : id(-1), rotation(ExactAngle::CLOCKWISE_0), scale(1.0f), confidence(0) {
        }

    private:
        /**
         * @brief Construct a match with a known pattern.
         * @param id         - ID of known marker that matches input.
         * @param rotation   - Rotation of detected marker relative to known marker.
         * @param scale      - The scale of the marker.
         * @param confidence - Confidence of match, interpretation depends on recognition method.
         */
        PatternMatch(int id, float rotation, float scale, float confidence)
            : id(id), rotation(rotation), scale(scale), confidence(confidence) {
        }
    };

    /**
     * @brief Recognizes markers from their contour and image.
     */
    class MarkerRecognizer {
    public:
        /**
         * @brief Constructs a recognizer for markers using the specified technique.
         * @param approach - Technique for recognizing markers.
         */
        explicit MarkerRecognizer(MarkerRecognitionApproach::Type approach = MarkerRecognitionApproach::HAMMING_DISTANCE)
            : approach(approach) {}

        /**
         * @brief Registers a known 6x6 binary pattern to match detected markers with.
         * @param id - Unique identifier for pattern, needs to be at least 0.
         * @param pattern - Pattern encoded as 6x6 black and white image.
         * @return True if the pattern is valid.
         */
        bool registerPattern(int id, const Mat& pattern);

        /**
         * @brief Recognize a single marker from its contour and source board image.
         * @param boardImage - Board image used for detecting the marker contour.
         * @param contour - Contour of the detected marker.
         * @return Closest match to detected marker, which can have a confidence of 0.
         * Match is only empty (id == -1) if no patterns have been registered.
         */
        PatternMatch recognizeMarker(const Mat& boardImage, const vector<Point>& contour) const;

    private:
        /// Technique for recognizing markers.
        MarkerRecognitionApproach::Type approach;

        /// Registered known patterns.
        std::map<int, Mat> patterns;

        /**
         * @brief Find the known pattern that best matches the detected pattern.
         * @param input - Detected pattern from extractPattern().
         * @return Info about best match or an empty match (id == -1) if no
         * patterns have been registered.
         */
        PatternMatch findMatchingPattern(const Mat& input) const;

        /**
         * @brief Rotates the detected marker to straighten it using its contour.
         * @param boardImage - Board image used for detecting the marker contour.
         * @param contour - Contour of the detected marker.
         * @return A tuple with the straightened and cropped marker, and the angle
         * used to straighten it.
         */
        static pair<Mat, float> straightenMarker(const Mat& boardImage, const vector<Point>& contour);

        /**
         * @brief Extracts the 6x6 binary pattern from a straightened marker.
         * @param straightenedMarker - Marker image from straightenMarker().
         * @return 6x6 binary image with the pattern.
         */
        static Mat extractPattern(const Mat& straightenedMarker);
    };

}

#endif
