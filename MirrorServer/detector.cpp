#include "detector.hpp"
#include <vector>

using std::vector;

namespace hierarchy_members {
    enum {
        NEXT,
        PREVIOUS,
        FIRST_CHILD,
        PARENT
    };
}

detector::detector(int captureDevice) {
    cap.open(captureDevice);

    width = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_WIDTH));
    height = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_HEIGHT));
}

void detector::setSurfaceCorners(const std::vector<Point2f>& corners) {
    surfaceCorners = corners;
}

void detector::detect(const detection_callback& callback) {
    // Process image
    Mat frame = capture();
    Mat correctedFrame = correctPerspective(frame);
    Mat thresholdedFrame = thresholdGreen(correctedFrame);

    // Use thresholded image to locate marker candidates
    int markersFound = locateMarkers(thresholdedFrame);

    callback(correctedFrame, markersFound);
}

void detector::loop(const detection_callback& callback) {
    while (true) {
        detect(callback);
    }
}

Mat detector::capture() {
    Mat frame;
    cap.read(frame);
    return frame;
}

Mat detector::correctPerspective(const Mat& rawFrame) const {
    static Point2f dst[] = {Point2f(0, 0), Point2f(640, 0), Point2f(0, 480), Point2f(640, 480)};
    Mat m = getPerspectiveTransform(surfaceCorners.data(), dst);

    Mat tmp, output;
    warpPerspective(rawFrame, tmp, m, cv::Size(width, height));
    resize(tmp, output, cv::Size(19 * 30, 24 * 30));

    return output;
}

Mat detector::thresholdGreen(const Mat& correctedFrame) const {
    // Split image into B, G, R components
    Mat frameParts[3];
    split(correctedFrame, frameParts);

    // Find parts of image that have a reasonable minimum green component,
    // don't contain much blue and have a red component > green
    Mat greenThreshold, blueThreshold;
    inRange(frameParts[1], cv::Scalar(50), cv::Scalar(255), greenThreshold);
    inRange(frameParts[0], cv::Scalar(60), cv::Scalar(255), blueThreshold);

    Mat rawThreshold = greenThreshold & ~blueThreshold & (frameParts[1] > frameParts[2]);

    // Clean up thresholded image by eroding noise and dilating to remove holes
    Mat tmp, cleanThreshold;
    auto kernel = getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    erode(rawThreshold, tmp, kernel);

    auto kernel2 = getStructuringElement(cv::MORPH_RECT, cv::Size(10, 10));
    dilate(tmp, cleanThreshold, kernel2);

    return cleanThreshold;
}

size_t detector::locateMarkers(const Mat& thresholdedFrame) const {
    vector<vector<Point>> contours;
    vector<Vec4i> hierarchy;

    // findContours mutates the input, so make a copy
    findContours(Mat(thresholdedFrame), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));

    // Find contours that look like markers (outer contour with exactly one inner contour)
    vector<size_t> potentialMarkers;

    for (size_t i = 0; i < hierarchy.size(); i++) {
        if (hierarchy[i][hierarchy_members::PARENT] < 0) {
            size_t children = 0;

            for (size_t j = 0; j < hierarchy.size(); j++) {
                if (hierarchy[j][hierarchy_members::PARENT] == i) {
                    children++;
                }
            }

            if (children == 1) {
                potentialMarkers.push_back(i);
            }
        }
    }

    return potentialMarkers.size();
}