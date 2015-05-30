#include "cornerdetector.hpp"

namespace mirrors {

CornerDetector::CornerDetector(const cv::Mat& image) {
    corners = findCorners(image);
}

const vector<Point>& CornerDetector::getCorners() {
    return corners;
}

vector<Point> CornerDetector::findCorners(const cv::Mat& image) {
    // Threshold on red
    Mat frameParts[3];
    split(image, frameParts);

    Mat mask = frameParts[2] > frameParts[1] * 2 & frameParts[2] > frameParts[0] * 2 & frameParts[2] > 50;
    Mat maskClean;
    Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, Size(3, 3));
    cv::erode(mask, maskClean, kernel);

    // Find 4 red corner regions and return them in the right order
    vector<vector<Point>> contours;
    cv::findContours(maskClean, contours, CV_RETR_LIST, CV_CHAIN_APPROX_NONE);

    if (contours.size() == 4) {
        return classifyCorners(contours);
    } else {
        return vector<Point>();
    }
}

vector<Point> CornerDetector::classifyCorners(const vector<vector<Point>>& contours) {
    // Find bounding regions.
    vector<cv::Rect> markerPoints;

    for (auto& points : contours) {
        markerPoints.push_back(cv::boundingRect(points));
    }

    // First sort by y to separate top and bottom markers.
    std::sort(markerPoints.begin(), markerPoints.end(), [](const cv::Rect& a, const cv::Rect& b) { return a.y < b.y; });

    vector<cv::Rect> top(markerPoints.begin(), markerPoints.begin() + 2);
    vector<cv::Rect> bottom(markerPoints.begin() + 2, markerPoints.begin() + 4);

    // Then sort each of them by x to find left and right
    std::sort(top.begin(), top.end(), [](const cv::Rect& a, const cv::Rect& b) { return a.x < b.x; });
    std::sort(bottom.begin(), bottom.end(), [](const cv::Rect& a, const cv::Rect& b) { return a.x < b.x; });

    // Determine the bounds of the board by taking the outer corners of the corner markers.
    vector<Point> corners = {
        Point(top[0].x, top[0].y), // Top-left
        Point(top[1].x + top[1].width, top[1].y), // Top-right
        Point(bottom[0].x, bottom[0].y + bottom[0].height), // Bottom-left
        Point(bottom[1].x + bottom[1].width, bottom[1].y + bottom[1].height) // Bottom-right
    };

    return corners;
}

}
