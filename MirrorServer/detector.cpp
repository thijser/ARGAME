#include "detector.hpp"
#include <vector>
#include <iostream>
#include <set>

using std::vector;

namespace hierarchy_members {
    enum {
        NEXT,
        PREVIOUS,
        FIRST_CHILD,
        PARENT
    };
}

detector::detector(int captureDevice, int requestedWidth, int requestedHeight)
        : keepGoing(true) {
    cap.open(captureDevice);

    cap.set(CV_CAP_PROP_FRAME_WIDTH, requestedWidth);
    cap.set(CV_CAP_PROP_FRAME_HEIGHT, requestedHeight);

    // Get actual resolution
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
    auto data = locateMarkers(thresholdedFrame);

    // Build collection of marker positions
    vector<Point> markerPositions;

    for (int contourIndex : data.candidates) {
        vector<Point> contour = data.contours[contourIndex];

        // Calculate center of marker
        Point sum;

        for (Point& p : contour) {
            sum += p;
        }

        sum.x = sum.x / contour.size();
        sum.y = sum.y / contour.size();

        markerPositions.push_back(sum);
    }

    callback(correctedFrame, markerPositions);
}

void detector::loop(const detection_callback& callback) {
    while (keepGoing && cv::waitKey(10) != 27) {
        detect(callback);
    }
    std::clog << "Detector stoppped" << std::endl;
}

void detector::stop() throw() {
    keepGoing = false;
}

Mat detector::capture() {
    Mat frame;
    cap.read(frame);
    return frame;
}

Mat detector::correctPerspective(const Mat& rawFrame) const {
    static Point2f dst[] = {Point2f(0, 0), Point2f(width, 0), Point2f(0, height), Point2f(width, height)};
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

    Mat rawThreshold = greenThreshold & (frameParts[1] > frameParts[2]) & (frameParts[1] > frameParts[0] * 1.2);

    // Clean up thresholded image by eroding noise and dilating to remove holes
    Mat tmp, cleanThreshold;
    auto kernel = getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    erode(rawThreshold, tmp, kernel);

    auto kernel2 = getStructuringElement(cv::MORPH_RECT, cv::Size(10, 10));
    dilate(tmp, cleanThreshold, kernel2);

    return cleanThreshold;
}

marker_locations detector::locateMarkers(const Mat& thresholdedFrame) const {
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
                if (hierarchy[j][hierarchy_members::PARENT] == static_cast<int>(i)) {
                    children++;
                }
            }

            if (children == 1) {
                potentialMarkers.push_back(i);
            }
        }
    }

    marker_locations data;
    data.contours = contours;
    data.hierarchy = hierarchy;
    data.candidates = potentialMarkers;

    return data;
}

// Source: http://opencv-code.com/quick-tips/how-to-rotate-image-in-opencv/
Mat detector::rotate(Mat src, double angle) {    
    int len = std::max(src.cols, src.rows);
    cv::Point2f pt(len / 2., len / 2.);
    cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);

    Mat dst;
    cv::warpAffine(src, dst, r, cv::Size(len, len));

    return dst;
}
