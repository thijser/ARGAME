#include "boarddetector.hpp"
#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace mirrors {

    using namespace cv;

    bool BoardDetector::locateBoard(const Mat& cameraImage) {
        auto markerContours = findMarkers(cameraImage);
        corners = classifyMarkers(markerContours);

        findBoardRatio(cameraImage);

        return !corners.empty();
    }

    cv::Size BoardDetector::getBoardSize() const {
        return boardSize;
    }

    void BoardDetector::findBoardRatio(const Mat& cameraImage) {
        if (boardRatio != -1) return;

        // Use this aspect ratio to get original camera image
        boardRatio = 1.0f;

        Mat tmp = extractBoard(cameraImage);

        if (tmp.rows != 0) {
            // Determine actual aspect ratio from corner marker size
            auto corners = findMarkers(tmp);
            cv::Rect bb = cv::boundingRect(corners[0]);
            boardRatio = bb.width / (float) bb.height;

            tmp = extractBoard(cameraImage);
            boardSize = Size(tmp.cols, tmp.rows);
        }
    }

    Mat BoardDetector::extractBoard(const Mat& cameraImage) const {
        // If the 4 corners haven't been found yet, there's nothing to do here.
        if (corners.empty()) {
            return Mat();
        }

        Point2f src[] = {
            corners[0],
            corners[1],
            corners[2],
            corners[3]
        };

        Point2f dst[] = {
            Point(0, 0),
            Point(cameraImage.cols, 0),
            Point(0, cameraImage.rows),
            Point(cameraImage.cols, cameraImage.rows)
        };

        Mat m = getPerspectiveTransform(src, dst);

        // Extract the board from the camera image and resize it to standardized size.
        Mat tmp, output;
        warpPerspective(cameraImage, tmp, m, cameraImage.size());

        // Resize to correct aspect ratio
        resize(tmp, output, cv::Size(tmp.cols / boardRatio, tmp.rows));

        return output;
    }

    vector<vector<Point>> BoardDetector::findMarkers(const Mat& cameraImage) const {
        // Convert image to HSV channels
        Mat imageHSV;
        cvtColor(cameraImage, imageHSV, CV_BGR2HSV);

        // Split image into H, S, V components
        Mat channels[3];
        split(imageHSV, channels);

        // Threshold on red
        Mat mask =
            // Significant red and light
            ((channels[HSV::H] > 160) | (channels[HSV::H] < 10)) &
            (channels[HSV::S] > 120) &
            (channels[HSV::V] > 50);

        Mat maskClean;

        if (approach == BoardDetectionApproach::RED_YELLOW_MARKERS) {
            // Remove noise
            Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, Size(5, 5));
            cv::morphologyEx(mask, maskClean, cv::MORPH_CLOSE, kernel);

            // Find contours that look like markers (first level inner contour)
            vector<vector<Point>> contours;
            vector<Vec4i> hierarchy;

            findContours(maskClean, contours, hierarchy, CV_RETR_TREE, CV_CHAIN_APPROX_NONE, Point(0, 0));

            vector<vector<Point>> potentialCorners;

            for (size_t i = 0; i < hierarchy.size(); i++) {
                int parent = hierarchy[i][HierarchyElement::PARENT];

                bool isInnerContour = parent >= 0;
                bool isFirstLevel = isInnerContour && hierarchy[parent][HierarchyElement::PARENT] < 0;

                cv::RotatedRect bb = cv::minAreaRect(contours[i]);
                bool largeEnough = bb.size.width >= 10 && bb.size.height >= 10;

                if (isInnerContour && isFirstLevel && largeEnough) {
                    potentialCorners.push_back(contours[parent]);
                }
            }

            return potentialCorners;
        } else {
            // Remove noise
            Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, Size(5, 5));
            cv::morphologyEx(mask, maskClean, cv::MORPH_OPEN, kernel);

            vector<vector<Point>> contours;
            cv::findContours(maskClean, contours, CV_RETR_LIST, CV_CHAIN_APPROX_NONE);

            return contours;
        }
    }

    vector<Point> BoardDetector::classifyMarkers(const vector<vector<Point>>& markerContours) const {
        // If there aren't exactly 4 marker contours, corner finding is not possible
        if (markerContours.size() != 4) {
            return vector<Point>();
        }

        // Find bounding regions of markers
        vector<cv::Rect> markerPoints;

        for (auto& points : markerContours) {
            markerPoints.push_back(cv::boundingRect(points));
        }

        // First sort by y to separate top and bottom markers
        std::sort(markerPoints.begin(), markerPoints.end(), [](const cv::Rect& a, const cv::Rect& b) { return a.y < b.y; });

        vector<cv::Rect> top(markerPoints.begin(), markerPoints.begin() + 2);
        vector<cv::Rect> bottom(markerPoints.begin() + 2, markerPoints.begin() + 4);

        // Then sort each of them by x to find left and right
        std::sort(top.begin(), top.end(), [](const cv::Rect& a, const cv::Rect& b) { return a.x < b.x; });
        std::sort(bottom.begin(), bottom.end(), [](const cv::Rect& a, const cv::Rect& b) { return a.x < b.x; });

        // Determine the bounds of the board by taking the outer corners of the corner markers
        // ____________
        // |_|       |_|
        // |           |
        // |_         _|
        // |_|_______|_|
        //
        vector<Point> corners = {
            Point(top[0].x, top[0].y), // Top-left
            Point(top[1].x + top[1].width, top[1].y), // Top-right
            Point(bottom[0].x, bottom[0].y + bottom[0].height), // Bottom-left
            Point(bottom[1].x + bottom[1].width, bottom[1].y + bottom[1].height) // Bottom-right
        };

        return corners;
    }

}
