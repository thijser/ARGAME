/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file markertracker.hpp
* @brief Contains implementation of mirrors::MarkerTracker.
*/

#ifndef MARKER_TRACKER_HPP
#define MARKER_TRACKER_HPP

#include "boarddetector.hpp"
#include "markerdetector.hpp"
#include "markerrecognizer.hpp"

#include <ctime>

namespace mirrors {

    /**
     * @brief Type of marker state update that occured.
     */
    namespace MarkerUpdateType {
        enum MarkerUpdateType {
            CHANGE
        };
    }

    class MarkerTracker;

    /**
     * @brief Info about a marker state update.
     */
    class MarkerUpdate {
        friend class MarkerTracker;

    public:
        /// Type of state update.
        MarkerUpdateType::MarkerUpdateType type;

        /// ID of detected pattern.
        int id;

        /// Pivot position of marker on board.
        Point position;

        /// Rotation of marker on board relative to pattern.
        float rotation;

    private:
        /**
         * @brief Construct a marker state change descriptor.
         * @param type - Type of state change.
         * @param id - ID of recognized pattern.
         * @param point - Pivot position of marker on board.
         * @param rotation - Rotation of marker relative to pattern.
         */
        MarkerUpdate(MarkerUpdateType::MarkerUpdateType type, int id, Point position, float rotation)
            : type(type), id(id), position(position), rotation(rotation) {}
    };

    /**
     * @brief Persistently tracks markers and changes to their state throughout frames.
     */
    class MarkerTracker {
    public:
        /**
         * @brief Construct a persistent tracker for markers using the specified detectors and recognizers.
         * The client is responsible for locating the board using boardDetector.
         * @param boardDetector - Detector to use to extract the board from a frame.
         * @param markerDetector - Detector to use to find markers on the board.
         * @param markerRecognizer - Recognizer to use to recognize markers on the board.
         */
        MarkerTracker(const BoardDetector& boardDetector, const MarkerDetector& markerDetector, const MarkerRecognizer& markerRecognizer)
            : boardDetector(boardDetector), markerDetector(markerDetector), markerRecognizer(markerRecognizer) {}

        /**
         * @brief Track markers in the next frame.
         * The timestamp is used for such things as recognizing disappeared markers.
         * @param frame - Next frame to track markers in.
         * @param timestamp - Timestamp of next frame.
         * @return Updates to marker state that occured in the next frame.
         */
        vector<MarkerUpdate> track(const Mat& frame, clock_t timestamp = clock());

    private:
        /// Detector for extracting the board from a frame.
        const BoardDetector& boardDetector;

        /// Detector for finding markers on the board.
        const MarkerDetector& markerDetector;

        /// Recognizer for markers on the board.
        const MarkerRecognizer& markerRecognizer;
    };

}

#endif