#include "trackermanager.hpp"

namespace mirrors {
    TrackerManager::TrackerManager(int captureDevice, Size requestedResolution, BoardDetectionApproach::Type boardApproach)
        : boardDetector(boardApproach), tracker(boardDetector, detector, recognizer) {

        cap = VideoCapture(captureDevice);

        // Request capture with specified resolution
        cap.set(CV_CAP_PROP_FRAME_WIDTH, requestedResolution.width);
        cap.set(CV_CAP_PROP_FRAME_HEIGHT, requestedResolution.height);

        // Retrieve and store actual resolution
        resolution.width = cap.get(CV_CAP_PROP_FRAME_WIDTH);
        resolution.height = cap.get(CV_CAP_PROP_FRAME_HEIGHT);
    }

    bool TrackerManager::loadPatterns(const std::string& directory, int count) {
        bool res = true;

        for (int i = 0; i < count; i++) {
            auto pattern = cv::imread(directory + "/" + std::to_string(i) + ".png", CV_LOAD_IMAGE_GRAYSCALE);
            res &= recognizer.registerPattern(i, pattern);
        }

        return res;
    }

    Size TrackerManager::getCaptureResolution() const {
        return resolution;
    }

    int TrackerManager::getUpdateRate() const {
        return fps;
    }

    bool TrackerManager::locateBoard(QPixmap& resultImage, bool infoOverlay) {
        Mat frame;
        cap.read(frame);

        if (infoOverlay) {
            Mat result = frame.clone();
            drawLocateBoardInstructions(result);
            resultImage = matToPixmap(result);
        } else {
            resultImage = matToPixmap(frame);
        }

        return boardDetector.locateBoard(frame);
    }

    std::vector<MarkerUpdate> TrackerManager::getMarkerUpdates(QPixmap& resultImage, bool infoOverlay) {
        Mat frame;
        cap.read(frame);

        Mat result = boardDetector.extractBoard(frame);

        // Try to track only if board has already been located
        vector<MarkerUpdate> updates;

        if (result.rows != 0) {
            updates = tracker.track(frame);

            if (infoOverlay) {
                drawTrackInfo(tracker, result, updates);
                resultImage = matToPixmap(result);
            }
        }

        // Update framerate
        if (time(nullptr) != latestSecond) {
            latestSecond = time(nullptr);
            fps = frames;
            frames = 0;
        } else {
            frames++;
        }

        return updates;
    }

    Point2f TrackerManager::scaledMarkerCoordinate(const Point& pos) const {
        float scale = tracker.getMarkerScale();
        return Point2f(pos.x / scale, pos.y / scale);
    }

    Point2f TrackerManager::scaledBoardSize() const {
        return scaledMarkerCoordinate(Point(boardDetector.getBoardSize()));
    }

    void TrackerManager::drawLocateBoardInstructions(Mat& resultImage) {
        const std::string boardLocatingInstructions = "Position the camera to view the entire board.";

        // Determine bounds of instruction text
        int baseLine;
        Size textSize = getTextSize(boardLocatingInstructions, FONT_HERSHEY_SIMPLEX, 0.5, 1, &baseLine);
        Point textPos = Point(resultImage.cols / 2, resultImage.rows / 2) + Point(-textSize.width / 2, textSize.height / 2);

        // Draw background rectangle
        int padding = 10;
        Point topLeft = Point(textPos.x - padding, textPos.y - textSize.height - padding);
        Point bottomRight = Point(textPos.x + textSize.width + padding, textPos.y + padding);
        cv::rectangle(resultImage, topLeft, bottomRight, cv::Scalar(0, 0, 0, 255), CV_FILLED);

        // Draw text itself
        putText(resultImage, boardLocatingInstructions, textPos, cv::FONT_HERSHEY_SIMPLEX, 0.5, cv::Scalar(255, 255, 255, 255), 1);
    }

    void TrackerManager::drawTrackInfo(const MarkerTracker& tracker, Mat& board, const vector<MarkerUpdate>& markers) {
        float scale = tracker.getMarkerScale();
        cv::Size markerSize(scale * 8, scale * 8);

        for (MarkerUpdate update : markers) {
            // Draw border around recognized marker
            cv::RotatedRect rrect(update.position, markerSize, update.rotation);
            cv::Point2f rrectPoints[4];
            rrect.points(rrectPoints);

            cv::Point rectPoints[4];
            for (int i = 0; i < 4; i++) {
                rectPoints[i] = rrectPoints[i];
            }

            // Draw solid color rotated square on recognized marker
            cv::fillConvexPoly(board, rectPoints, 4, cv::Scalar(0, 0, 0, 128));

            // Draw centered text with recognized ID in it
            int baseLine;
            cv::Size textSize = cv::getTextSize(std::to_string(update.id), cv::FONT_HERSHEY_SIMPLEX, 1.2, 2, &baseLine);

            Point textPos = update.position + Point(-textSize.width / 2, textSize.height / 2);

            cv::putText(board, std::to_string(update.id), textPos, cv::FONT_HERSHEY_SIMPLEX, 1.2, cv::Scalar(255, 255, 255, 255), 2);
        }
    }

    QPixmap TrackerManager::matToPixmap(Mat& input) {
        // Convert image to RGB format to create QImage from it
        cv::Mat copy;
        cv::cvtColor(input, copy, CV_BGR2RGB);
        QImage image(copy.data, copy.cols, copy.rows, static_cast<int>(copy.step), QImage::Format_RGB888);

        // Create pixmap from it
        return QPixmap::fromImage(image, Qt::ColorOnly);
    }
}
