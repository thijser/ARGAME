#ifndef TRACKERMANAGER_HPP
#define TRACKERMANAGER_HPP

#include <opencv2/core.hpp>
#include <opencv2/videoio.hpp>
#include <QPixmap>
#include "markertracker.hpp"

namespace mirrors {
    using namespace cv;

    /**
     * @brief Controls tracking of marker and returns the results.
     */
    class TrackerManager {
    public:
        /**
         * @brief Constructs tracker manager with given camera, resolution and board detection approach.
         * @param captureDevice - Id of camera device to use.
         * @param requestedResolution - Resolution to try and capture with.
         * @param boardApproach - Approach to use to find board in camera image.
         */
        TrackerManager(int captureDevice, Size requestedResolution, BoardDetectionApproach::Type boardApproach);

        /**
         * @brief Load *count* patterns from the specified directory.
         * @param directory - Directory to load patterns from.
         * @param count - Number of markers to load (directory/##.png).
         * @return Boolean indicating if all patterns were successfully loaded.
         */
        bool loadPatterns(const std::string& directory, int count);

        /**
         * @brief Retrieve the actual capture resolution.
         * @return Actual capture resolution for camera.
         */
        Size getCaptureResolution() const;

        /**
         * @brief Get the rate at which getMarkerUpdates() finishes per second.
         * @return Updates per second.
         */
        int getUpdateRate() const;

        /**
         * @brief Find the board.
         * @param resultImage - Image of camera (with board finding instructions).
         * @param infoOverlay - Render board finding instructions on output image.
         * @return Camera image with or without overlay.
         */
        bool locateBoard(QPixmap& resultImage, bool infoOverlay);

        /**
         * @brief Detect marker changes.
         * @param resultImage - Image of board (with marker tracking info).
         * @param infoOverlay - Render marker tracking overlay on output image.
         * @return Board image with or without overlay.
         */
        vector<MarkerUpdate> getMarkerUpdates(QPixmap& resultImage, bool infoOverlay);

        /**
         * @brief Scale pixel coordinates to marker units.
         * 1 marker unit = width/height of detected marker patterns
         * @param pos - Pixel coordinate.
         * @return Pixel coordinate in marker units.
         */
        Point2f scaledMarkerCoordinate(const Point& pos) const;

        /**
         * @brief Retrieve size of board in marker units.
         * @return Board size in marker units.
         */
        Point2f scaledBoardSize() const;

    private:
        /// Capture device to retrieve frames from.
        VideoCapture cap;

        /// Actual resolution used for capturing.
        Size resolution;

        /// Current second.
        time_t latestSecond;

        /// Update frames so far and total amount in the previous second.
        int frames, fps;

        /// Detectors, recognizers and trackers to use.
        BoardDetector boardDetector;
        MarkerDetector detector;
        MarkerRecognizer recognizer;
        MarkerTracker tracker;

        /**
         * @brief Draw board locating instructions on top of camera image.
         * @param resultImage - Camera image and resulting image (in/out).
         */
        static void drawLocateBoardInstructions(Mat& resultImage);

        /**
         * @brief Draw marker tracking info on top of board image.
         * @param tracker - Marker tracker used for tracking markers.
         * @param board - Image of board and resulting image (in/out).
         * @param markers - Marker updates.
         */
        static void drawTrackInfo(const MarkerTracker& tracker, Mat& board, const vector<MarkerUpdate>& markers);

        /**
         * @brief Convert OpenCV image to Qt pixmap.
         * @param input - OpenCV image.
         * @return QPixmap representation of OpenCV image.
         */
        static QPixmap matToPixmap(Mat& input);
    };
}

#endif // TRACKERMANAGER_HPP

