#ifndef TRACKERMANAGER_HPP
#define TRACKERMANAGER_HPP

#include <opencv2/core.hpp>
#include <opencv2/videoio.hpp>
#include "markertracker.hpp"

// TODO: Resize image, draw overlay and then output QPixmap

namespace mirrors {
    using namespace cv;

    class TrackerManager {
    public:
        TrackerManager(int captureDevice, Size requestedResolution, BoardDetectionApproach::Type boardApproach);

        bool loadPatterns(const std::string& directory, int count);

        Size getCaptureResolution() const;

        int getUpdateRate() const;

        bool locateBoard(Mat& resultImage, bool infoOverlay);

        vector<MarkerUpdate> getMarkerUpdates(Mat& resultImage, bool infoOverlay);

        Point2f scaledMarkerCoordinate(const MarkerUpdate& update);

    private:
        VideoCapture cap;
        Size resolution;

        time_t latestSecond;
        int frames, fps;

        BoardDetector boardDetector;
        MarkerDetector detector;
        MarkerRecognizer recognizer;
        MarkerTracker tracker;

        static void drawLocateBoardInstructions(Mat& resultImage);

        static void drawTrackInfo(const MarkerTracker& tracker, Mat& board, const vector<MarkerUpdate>& markers);
    };
}

#endif // TRACKERMANAGER_HPP

