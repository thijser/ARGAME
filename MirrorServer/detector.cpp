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

void detector::detect(const detection_callback& callback) {
    // Capture image from camera
    Mat frame = capture();

    auto corners = getAveragedCorners(frame);

    if (corners.size() == 4) {
        Mat correctedFrame = correctPerspective(frame, corners);
        Mat thresholdedFrame = thresholdGreen(correctedFrame);

        // Use thresholded image to locate marker candidates
        auto data = locateMarkers(thresholdedFrame);

        // Isolate code of first marker
        Mat markerBig = findMarker(correctedFrame, data);

        // Build collection of marker positions
        vector<Point> markerPositions;

        for (int contourIndex : data.candidates) {
            // Calculate center of marker
            vector<Point> contour = data.contours[contourIndex];
            markerPositions.push_back(averageOfPoints(contour));
        }

        callback(correctedFrame, markerPositions);
    } else {
        callback(frame, vector<Point>());
    }
}

vector<Point2f> detector::getAveragedCorners(const Mat& rawFrame) {
    // Detect corners of current frame and add them to the history
    auto newCorners = findCorners(rawFrame);
    if (newCorners.size() == 4) {
        cornersHistory.push_back(newCorners);
    }

    // Keep history of last <cornerMovingAverageHistory> corners
    if (cornersHistory.size() > cornerMovingAverageHistory) {
        cornersHistory.erase(cornersHistory.begin());
    } else if (cornersHistory.size() == 0) {
        return vector<Point2f>();
    }

    // Calculate average for each corner and return it
    vector<Point> topLeft;
    vector<Point> topRight;
    vector<Point> bottomLeft;
    vector<Point> bottomRight;

    for (auto& corners : cornersHistory) {
        topLeft.push_back(corners[0]);
        topRight.push_back(corners[1]);
        bottomLeft.push_back(corners[2]);
        bottomRight.push_back(corners[3]);
    }

    return {
        averageOfPoints(topLeft),
        averageOfPoints(topRight),
        averageOfPoints(bottomLeft),
        averageOfPoints(bottomRight)
    };
}

vector<Point2f> detector::findCorners(const Mat& rawFrame) const {
    // Threshold on red
    Mat frameParts[3];
    split(rawFrame, frameParts);

    Mat mask = frameParts[2] > frameParts[1] * 2 & frameParts[2] > frameParts[0] * 2 & frameParts[2] > 50;
    Mat maskClean;
    Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, Size(3, 3));
    cv::erode(mask, maskClean, kernel);

    // Find 4 red corner regions
    vector<vector<Point>> contours;
    cv::findContours(maskClean, contours, CV_RETR_LIST, CV_CHAIN_APPROX_NONE);

    // Find center points
    vector<Point> markerPoints;

    for (auto& points : contours) {
        markerPoints.push_back(averageOfPoints(points));
    }

    if (contours.size() == 4) {
        // First sort by y to separate top and bottom markers
        std::sort(markerPoints.begin(), markerPoints.end(), [](Point a, Point b) { return a.y < b.y; });

        vector<Point> topMarkers(markerPoints.begin(), markerPoints.begin() + 2);
        vector<Point> bottomMarkers(markerPoints.begin() + 2, markerPoints.begin() + 4);

        // Then sort each of them by x to find left and right
        std::sort(topMarkers.begin(), topMarkers.end(), [](Point a, Point b) { return a.x < b.x; });
        std::sort(bottomMarkers.begin(), bottomMarkers.end(), [](Point a, Point b) { return a.x < b.x; });

        vector<Point2f> corners;
        corners.push_back(topMarkers[0]); // Top-left
        corners.push_back(topMarkers[1]); // Top-right
        corners.push_back(bottomMarkers[0]); // Bottom-left
        corners.push_back(bottomMarkers[1]); // Bottom-right
        
        return corners;
    }

    return vector<Point2f>();
}

Mat detector::findMarker(const Mat& correctedFrame, const marker_locations& data) const {
    // Find rotated bounding rect of marker
    if (data.candidates.empty()) return correctedFrame;
    auto brect = cv::boundingRect(data.contours[data.candidates[0]]);

    cv::RotatedRect rect = cv::minAreaRect(data.contours[data.candidates[0]]);

    Mat marker = correctedFrame(brect);
    marker = rotate(marker, rect.angle);

    // Cut off border
    int w = static_cast<int>(rect.size.width) - 20;
    int h = static_cast<int>(rect.size.height) - 20;

    if (w > 7 && h > 7 && w <= marker.size[0] && h <= marker.size[1] && w > 0 && h > 0) {
        cv::Rect roi;
        roi.x = marker.size[0] / 2 - w / 2;
        roi.y = marker.size[1] / 2 - h / 2;
        roi.width = w;
        roi.height = h;
        marker = marker(roi);

        // Threshold for grayish area to find black/white region
        Mat markerParts[3];
        split(marker, markerParts);

        Mat mask = ~(markerParts[1] > 50 & (markerParts[1] > markerParts[2]) & (markerParts[1] > markerParts[0] * 1.2));

        vector<vector<Point>> contours;
        cv::findContours(mask, contours, CV_RETR_LIST, CV_CHAIN_APPROX_NONE);

        if (contours.size() > 0) {
            // FIXME: largestPoints will only ever be 0
            size_t largestPoints = 0;
            cv::Rect bb;

            for (auto& contour : contours) {
                if (contour.size() > largestPoints) {
                    bb = cv::boundingRect(contour);

                    if (bb.width > 3 && bb.height > 3) {
                        bb.x += 3;
                        bb.y += 3;
                        bb.width -= 6;
                        bb.height -= 6;
                    }
                }
            }

            if (bb.x >= 0 && bb.y >= 0 && bb.width > 0 && bb.height > 0 && bb.x + bb.width < marker.size[0] && bb.y + bb.height < marker.size[1]) {
                Mat codeImage = marker(bb);

                // Turn into grayscale and threshold to find black and white code
                Mat codeImageGray, thresholdedCode, downsizedCode;
                cv::cvtColor(codeImage, codeImageGray, CV_BGR2GRAY);
                cv::resize(codeImageGray, downsizedCode, Size(6, 6), 0, 0, cv::INTER_LINEAR);

                int avgPixel = 0;

                for (int i = 0; i < 6; i++) {
                    for (int j = 0; j < 6; j++) {
                        avgPixel += downsizedCode.at<uint8_t>(i, j);
                    }
                }

                avgPixel /= 36;

                cv::threshold(downsizedCode, thresholdedCode, avgPixel, 255, 0);

                marker = thresholdedCode;
            }
        }
    }

    Mat markerBig;
    cv::resize(marker, markerBig, Size(marker.size[0] * 32, marker.size[1] * 32), 0, 0, cv::INTER_NEAREST);

    return markerBig;
}

void detector::loop(const detection_callback& callback) {
    while (keepGoing) {
        detect(callback);
        int keyCode = cv::waitKey(10);
        if (keyCode == 27 || keyCode == 113) {
            stop();
        }
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

Mat detector::correctPerspective(const Mat& rawFrame, const vector<Point2f>& corners) const {
    static Point2f dst[] = {Point2f(0, 0), Point2f(width, 0), Point2f(0, height), Point2f(width, height)};
    Mat m = getPerspectiveTransform(corners.data(), dst);

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

Point detector::averageOfPoints(const vector<Point>& points) {
    Point sum;

    for (auto& p : points) {
        sum += p;
    }

    sum.x = sum.x / points.size();
    sum.y = sum.y / points.size();

    return sum;
}

// Source: http://opencv-code.com/quick-tips/how-to-rotate-image-in-opencv/
Mat detector::rotate(Mat src, double angle) {    
    int len = std::max(src.cols, src.rows);
    cv::Point2f pt(len / 2.f, len / 2.f);
    cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);

    Mat dst;
    cv::warpAffine(src, dst, r, cv::Size(len, len));

    return dst;
}
