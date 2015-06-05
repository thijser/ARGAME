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
        trackFastMarkers(frameMarkers, timestamp, updates);

        for (auto& trackedMarker : trackedMarkers) {
            trackedMarker.newThisFrame = false;
            trackedMarker.seenThisFrame = false;
        }

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
            TrackedMarker* closest = findClosestMarker(detectedMarker.first, MARKER_MAX_FRAME_DIST);

            if (closest != nullptr) {
                MarkerUpdateType::MarkerUpdateType updateType = MarkerUpdateType::CHANGE;

                closest->lastSighting = timestamp;
                closest->seenThisFrame = true;

                closest->velocity = dist(closest->position, detectedMarker.first);

                // Update the rotation if the newest pattern match is the same as the current one
                if (detectedMarker.second.id == closest->match.id) {
                    closest->match.rotation = detectedMarker.second.rotation;
                }

                // If the new match is more confidence, completely replace the current match
                if (closest->velocity <= MARKER_MAX_RECOGNITION_VELOCITY &&
                    detectedMarker.second.confidence > closest->match.confidence) {

                    // But first log a delete for the old marker (if the detected pattern has changed)
                    if (closest->match.id != detectedMarker.second.id) {
                        // And if it was ever detected at all
                        if (closest->match.id != -1) {
                            updates.push_back(MarkerUpdate(MarkerUpdateType::REMOVE, closest->position, closest->rotation, closest->match));
                        }

                        updateType = MarkerUpdateType::NEW;
                    }

                    closest->match = detectedMarker.second;
                }

                // Smoothen position
                closest->position = closest->positions.update(detectedMarker.first);

                // Smoothen rotation
                closest->rotation = closest->rotations.update(closest->match.rotation);

                // Finally, log a CHANGE or NEW depending on a change of detected pattern
                if (closest->match.id != -1) {
                    updates.push_back(MarkerUpdate(updateType, closest->position, closest->rotation, closest->match));
                }
            }
        }
    }

    void MarkerTracker::trackNewMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates) {
        for (auto& detectedMarker : detectedMarkers) {
            TrackedMarker* closestMarker = findClosestMarker(detectedMarker.first, MARKER_MAX_FRAME_DIST);

            if (closestMarker == nullptr) {
                TrackedMarker newMarker(timestamp, detectedMarker.first, detectedMarker.second);

                if (detectedMarker.second.confidence >= MARKER_MIN_CONFIDENCE) {
                    updates.push_back(MarkerUpdate(MarkerUpdateType::NEW, detectedMarker.first, newMarker.rotation, detectedMarker.second));
                }

                trackedMarkers.push_back(newMarker);
            }
        }
    }

    void MarkerTracker::trackRemovedMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates) {
        Q_UNUSED(detectedMarkers);
        vector<TrackedMarker> newTrackedMarkers;

        // Replace tracked markers list with only markers that have been seen recently
        // and return remove updates for the timed out ones
        for (auto& trackedMarker : trackedMarkers) {
            if (timestamp - trackedMarker.lastSighting < MARKER_TIMEOUT_TIME) {
                newTrackedMarkers.push_back(trackedMarker);
            } else {
                updates.push_back(MarkerUpdate(MarkerUpdateType::REMOVE, trackedMarker.position, trackedMarker.rotation, trackedMarker.match));
            }
        }

        trackedMarkers = newTrackedMarkers;
    }

    void MarkerTracker::trackFastMarkers(const vector<pair<Point, PatternMatch>>& detectedMarkers, clock_t timestamp, vector<MarkerUpdate>& updates) {
        Q_UNUSED(detectedMarkers);
        Q_UNUSED(timestamp);
        // Check if special condition applies
        int newMarkerCount = 0;
        int removedMarkerCount = 0;

        for (auto& trackedMarker : trackedMarkers) {
            if (trackedMarker.newThisFrame) newMarkerCount++;
            else if (!trackedMarker.seenThisFrame) removedMarkerCount++;
        }

        // If exactly 1 marker was removed and 1 new marker appeared within
        // this single frame, assume that it moved very quickly.
        if (newMarkerCount == 1 && removedMarkerCount == 1) {
            for (auto& trackedMarker : trackedMarkers) {
                if (!trackedMarker.seenThisFrame && !trackedMarker.newThisFrame) {
                    trackedMarker.position = trackedMarker.positions.update(trackedMarkers.back().position);
                    trackedMarker.lastSighting = clock();

                    updates.push_back(MarkerUpdate(MarkerUpdateType::CHANGE, trackedMarker.position, trackedMarker.rotation, trackedMarker.match));

                    break;
                }
            }

            // Remove erroneously detected new marker and the last NEW update
            // (We can't just remove the first one, because it could be produced
            // by trackChangedMarkers.)
            trackedMarkers.pop_back();

            int lastNewUpdate = -1;

            for (size_t i = 0; i < updates.size(); i++) {
                if (updates[i].type == MarkerUpdateType::NEW) {
                    lastNewUpdate = static_cast<int>(i);
                }
            }

            // It's possible that there is no NEW update if the marker hasn't
            // been recognized yet.
            if (lastNewUpdate != -1) {
                updates.erase(updates.begin() + lastNewUpdate);
            }
        }
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
