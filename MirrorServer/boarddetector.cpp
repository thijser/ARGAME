#include "boarddetector.hpp"
#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace mirrors {

    using cv::Point2f;
    using cv::Size;

    bool BoardDetector::locateBoard(const Mat& cameraImage) {
        auto markerContours = findMarkers(cameraImage);
        corners = classifyMarkers(markerContours);

        return !corners.empty();
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
        resize(tmp, output, cv::Size(19 * 30, 24 * 30));

        return output;
    }

    vector<vector<Point>> BoardDetector::findMarkers(const Mat& cameraImage) const {
        // Threshold on red
        Mat channels[3];
        cv::split(cameraImage, channels);

        Mat mask =
            // Significant red
            channels[BGR::R] > 50 &
            // Significantly higher red channel than other channels
            channels[BGR::R] > channels[BGR::G] * 2 &
            channels[BGR::R] > channels[BGR::B] * 2;

        // Remove noise
        Mat maskClean;
        Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, Size(3, 3));
        cv::morphologyEx(mask, maskClean, cv::MORPH_OPEN, kernel);

        vector<vector<Point>> contours;
        cv::findContours(maskClean, contours, CV_RETR_LIST, CV_CHAIN_APPROX_NONE);

        return contours;
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