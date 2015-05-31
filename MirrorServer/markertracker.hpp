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
#include "averager.hpp"

#include <ctime>

namespace mirrors {

    /// Maximum distance a marker can move per frame before it's considered a new marker.
    const float MARKER_MAX_FRAME_DIST = 50;

    /// Minimum confidence for marker recognition before a marker is considered valid.
    const float MARKER_MIN_CONFIDENCE = 0.7f;

    /// Maximum speed before recognition of marker pattern is disabled (pixels/frame)
    const float MARKER_MAX_RECOGNITION_VELOCITY = 15;

    /// Time of not seeing a marker before it is considered removed.
    const clock_t MARKER_TIMEOUT_TIME = CLOCKS_PER_SEC / 2;

    /// Amount of frames to average for marker positions and rotations.
    const int MARKER_HISTORY_LENGTH = 15;

    /// Minimum distance (pixels) between smoothed position and new position before
    /// smoothing is disabled and the new position is assumed directly.
    const float MARKER_POSITION_SMOOTH_THRESHOLD = 3;

    /// Minimum distance (pixels) between smoothed rotation and new rotation before
    /// smoothing is disabled and the new rotation is assumed directly.
    const float MARKER_ROTATION_SMOOTH_THRESHOLD = 3;

    /**
     * @brief Type of marker state update that occured.
     */
    namespace MarkerUpdateType {
        enum MarkerUpdateType {
            NEW,
            CHANGE,
            REMOVE
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
         * @param position - Pivot position of marker on board.
         * @param rotation - Rotation of marker relative to detected pattern.
         * @param match - Match result of marker (used for id and rotation).
         */
        MarkerUpdate(MarkerUpdateType::MarkerUpdateType type, Point position, float rotation, PatternMatch match)
            : type(type), id(match.id), position(position), rotation(rotation) {}
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

        /**
         * @brief Describes state of persistently tracked marker.
         */
        struct TrackedMarker {
            /// Last time this marker was seen.
            clock_t lastSighting;

            /// Last known positions of marker.
            Averager<Point> positions = Averager<Point>(MARKER_HISTORY_LENGTH, MARKER_POSITION_SMOOTH_THRESHOLD);

            /// Smoothed position of marker.
            Point position;

            /// Last known rotations of marker.
            Averager<float> rotations = Averager<float>(MARKER_HISTORY_LENGTH, MARKER_ROTATION_SMOOTH_THRESHOLD);

            /// Smoothed rotation of marker.
            float rotation;

            /// Last known velocity of marker (pixels/frame).
            float velocity;

            /// Most confident pattern match of marker thus far.
            PatternMatch match;

            /// Flag indicating if this marker was added this frame.
            bool newThisFrame = true;

            /// Flag indicating if this marker was (first) seen in this frame.
            bool seenThisFrame = true;

            /**
             * @brief Creates a state descriptor for a new persistent marker.
             * @param timestamp - First time this marker was detected.
             * @param position - First position of this marker.
             * @param match - First pattern match results for this marker.
             */
            TrackedMarker(clock_t timestamp, Point position, PatternMatch match = PatternMatch())
                : lastSighting(timestamp), match(match) {

                this->position = positions.update(position);
                this->rotation = rotations.update(match.rotation);
            }
        };

        /// List of persistently tracked markers.
        vector<TrackedMarker> trackedMarkers;

        /**
         * @brief Detects and recognizes all the markers in a new frame.
         * @param frame - New frame to analyze.
         * @return List of recognized markers as (position, match) tuples.
         */
        vector<pair<Point, PatternMatch>> detectMarkers(const Mat& frame) const;

        /**
         * @brief Updates the persistent marker state with regards to changed markers.
         * @param detectedMarkers - Markers detected using detectMarkers().
         * @param timestamp - Timestamp at which markers were detected.
         * @param updates - List of marker updates to write changes to.
         */
        void trackChangedMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates);

        /**
        * @brief Updates the persistent marker state with regards to new markers.
        * Must be called after trackChangedMarkers().
        * @param detectedMarkers - Markers detected using detectMarkers().
        * @param timestamp - Timestamp at which markers were detected.
        * @param updates - List of marker updates to write changes to.
        */
        void trackNewMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates);

        /**
        * @brief Updates the persistent marker state with regards to removed markers.
        * Must be called after trackNewMarkers().
        * @param detectedMarkers - Markers detected using detectMarkers().
        * @param timestamp - Timestamp at which markers were detected.
        * @param updates - List of marker updates to write changes to.
        */
        void trackRemovedMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates);

        /**
         * @brief Updates the persistent marker state to track fast markers in a special case.
         * If during a single frame, 1 marker has disappeared (not timed out) and 1 new marker
         * has appeared, we assume that it's the same marker that has simply moved very quickly.
         * Must be called after trackRemovedMarkers().
         * @param detectedMarkers - Markers detected using detectMarkers().
         * @param timestamp - Timestamp at which markers were detected.
         * @param updates - List of marker updates to write changes to.
         */
        void trackFastMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates);

        /**
         * @brief Finds the tracked marker closest to the specified point.
         * @param point - Point to find closest marker center around.
         * @param maxDist - Optional parameter to limit the search radius (pixels).
         * @return Reference to closest marker in trackedMarkers list or nullptr if
         * none was found in the given range.
         */
        TrackedMarker* findClosestMarker(const Point& point, float maxDist = std::numeric_limits<float>::infinity());
    };

}

#endif