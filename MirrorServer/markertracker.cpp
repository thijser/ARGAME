#include "markertracker.hpp"

namespace mirrors {

    vector<MarkerUpdate> MarkerTracker::track(const Mat& frame, clock_t timestamp) {
        auto boardImage = boardDetector.extractBoard(frame);
        auto markerContours = markerDetector.locateMarkers(boardImage);

        vector<MarkerUpdate> updates;
        
        for (auto& contour : markerContours) {
            auto patternMatch = markerRecognizer.recognizeMarker(boardImage, contour);

            if (patternMatch.confidence >= 0.70f) {
                updates.push_back(MarkerUpdate(MarkerUpdateType::CHANGE, patternMatch.id, getPivot(contour), patternMatch.rotation));
            }
        }

        return updates;
    }

}