#ifndef TRACKERMANAGER_HPP
#define TRACKERMANAGER_HPP

#include <opencv2/core.hpp>
#include <opencv2/videoio.hpp>
#include <QPixmap>
#include "markertracker.hpp"

namespace mirrors {
    using namespace cv;

    class TrackerManager {
    public:
        TrackerManager(int captureDevice, Size requestedResolution, BoardDetectionApproach::Type boardApproach);

        bool loadPatterns(const std::string& directory, int count);

        Size getCaptureResolution() const;

        int getUpdateRate() const;

        bool locateBoard(QPixmap& resultImage, bool infoOverlay);

        vector<MarkerUpdate> getMarkerUpdates(QPixmap& resultImage, bool infoOverlay);

        Point2f scaledMarkerCoordinate(const Point& pos) const;

        Point2f scaledBoardSize() const;

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

        static QPixmap matToPixmap(Mat& input);
    };
}

#endif // TRACKERMANAGER_HPP

