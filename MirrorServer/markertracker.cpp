#include "markertracker.hpp"

namespace mirrors {

    vector<MarkerUpdate> MarkerTracker::track(const Mat& frame, clock_t timestamp) {
        // Build list of markers detected in this frame
        auto frameMarkers = detectMarkers(frame);

        // Use it to update marker states and produce state updates for client
        vector<MarkerUpdate> updates;

        trackChangedMarkers(frameMarkers, timestamp, updates);
        trackNewMarkers(frameMarkers, timestamp, updates);
        trackRemovedMarkers(frameMarkers, timestamp, updates);

        return updates;
    }

    vector<pair<Point, PatternMatch>> MarkerTracker::detectMarkers(const Mat& frame) const {
        auto boardImage = boardDetector.extractBoard(frame);
        auto markerContours = markerDetector.locateMarkers(boardImage);

        vector<pair<Point, PatternMatch>> detectedMarkers;

        for (auto& contour : markerContours) {
            auto patternMatch = markerRecognizer.recognizeMarker(boardImage, contour);

            detectedMarkers.push_back(std::make_pair(getPivot(contour), patternMatch));
        }
        
        return detectedMarkers;
    }

    void MarkerTracker::trackChangedMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates) {
        for (auto& detectedMarker : detectedMarkers) {
            TrackedMarker* closestMarker = findClosestMarker(detectedMarker.first, MARKER_MAX_FRAME_DIST);

            if (closestMarker != nullptr) {
                MarkerUpdateType::MarkerUpdateType updateType = MarkerUpdateType::CHANGE;

                closestMarker->lastSighting = timestamp;

                closestMarker->velocity = dist(closestMarker->position, detectedMarker.first);
                closestMarker->position = detectedMarker.first;

                // Update the rotation if the newest pattern match is the same as the current one
                if (detectedMarker.second.id == closestMarker->match.id) {
                    closestMarker->match.rotation = detectedMarker.second.rotation;
                }

                // If the new match is more confidence, completely replace the current match
                if (closestMarker->velocity <= MARKER_MAX_RECOGNITION_VELOCITY &&
                    detectedMarker.second.confidence > closestMarker->match.confidence) {

                    // But first log a delete for the old marker (if the detected pattern has changed)
                    if (closestMarker->match.id != detectedMarker.second.id) {
                        updates.push_back(MarkerUpdate(MarkerUpdateType::REMOVE, detectedMarker.first, detectedMarker.second));
                        updateType = MarkerUpdateType::NEW;
                    }

                    closestMarker->match = detectedMarker.second;
                }

                // Finally, log a CHANGE or NEW depending on a change of detected pattern
                updates.push_back(MarkerUpdate(updateType, detectedMarker.first, detectedMarker.second));
            }
        }
    }

    void MarkerTracker::trackNewMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates) {
        for (auto& detectedMarker : detectedMarkers) {
            TrackedMarker* closestMarker = findClosestMarker(detectedMarker.first, MARKER_MAX_FRAME_DIST);

            if (closestMarker == nullptr) {
                TrackedMarker newMarker(timestamp, detectedMarker.first, detectedMarker.second);
                trackedMarkers.push_back(newMarker);

                if (detectedMarker.second.confidence >= MARKER_MIN_CONFIDENCE) {
                    updates.push_back(MarkerUpdate(MarkerUpdateType::NEW, detectedMarker.first, detectedMarker.second));
                }
            }
        }
    }

    void MarkerTracker::trackRemovedMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates) {
        vector<TrackedMarker> newTrackedMarkers;

        // Replace tracked markers list with only markers that have been seen recently
        for (auto& trackedMarker : trackedMarkers) {
            if (timestamp - trackedMarker.lastSighting < MARKER_TIMEOUT_TIME) {
                newTrackedMarkers.push_back(trackedMarker);
            } else {
                updates.push_back(MarkerUpdate(MarkerUpdateType::REMOVE, trackedMarker.position, trackedMarker.match));
            }
        }

        trackedMarkers = newTrackedMarkers;
    }

    MarkerTracker::TrackedMarker* MarkerTracker::findClosestMarker(const Point& point, float maxDist) {
        TrackedMarker* closestMarker = nullptr;

        for (auto& trackedMarker : trackedMarkers) {
            float d = dist(trackedMarker.position, point);

            if ((closestMarker == nullptr || d < dist(closestMarker->position, point)) && d <= maxDist) {
                closestMarker = &trackedMarker;
            }
        }

        return closestMarker;
    }

}