/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

#include "detector.hpp"
#include <vector>
#include <iostream>
#include <set>

namespace mirrors {

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
    : keepGoing(true), cornersHistory(ringbuffer<vector<Point2f>>(CORNER_HISTORY_LENGTH)) {
    cap.open(captureDevice);

    cap.set(CV_CAP_PROP_FRAME_WIDTH, requestedWidth);
    cap.set(CV_CAP_PROP_FRAME_HEIGHT, requestedHeight);

    // Get actual resolution
    width = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_WIDTH));
    height = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_HEIGHT));
}

bool detector::registerMarkers(const vector<Mat>& markers) {
    for (auto& marker : markers) {
        if (marker.channels() != 1 || marker.rows != 6 || marker.cols != 6) {
            return false;
        } else {
            cv::threshold(marker, marker, 128, 255, 0);
        }
    }

    markerPatterns.insert(markerPatterns.end(), markers.begin(), markers.end());

    return true;
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

        // Recognize markers using codes
        auto markers = recognizeMarkers(correctedFrame, data);

        // Build collection of marker positions
        vector<Point> markerPositions;

        for (int contourIndex : data.candidates) {
            // Calculate center of marker
            vector<Point> contour = data.contours[contourIndex];
            markerPositions.push_back(averageOfPoints(contour));
        }

        callback(correctedFrame, markers);
    } else {
        callback(frame, vector<detected_marker>());
    }
}

vector<Point2f> detector::getAveragedCorners(const Mat& rawFrame) {
    // Detect corners of current frame and add them to the history
    auto newCorners = findCorners(rawFrame);
    if (newCorners.size() == 4) {
        cornersHistory.add(newCorners);
    }

    // Keep history of last <cornerMovingAverageHistory> corners
    if (cornersHistory.size() == 0) {
        return vector<Point2f>();
    }

    // Calculate average for each corner and return it
    vector<Point> topLeft;
    vector<Point> topRight;
    vector<Point> bottomLeft;
    vector<Point> bottomRight;

    for (auto& corners : cornersHistory.data()) {
        topLeft.push_back(corners[0]);
        topRight.push_back(corners[1]);
        bottomLeft.push_back(corners[2]);
        bottomRight.push_back(corners[3]);
    }

    return{
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

vector<detected_marker> detector::recognizeMarkers(const Mat& correctedFrame, const marker_locations& data) {
    vector<detected_marker> markers;

    for (int contourId : data.candidates) {
        auto brect = cv::boundingRect(data.contours[contourId]);

        cv::RotatedRect rotatedRect = cv::minAreaRect(data.contours[contourId]);

        Mat marker = correctedFrame(brect);
        marker = rotate(marker, rotatedRect.angle);

        // Cut off border
        int w = static_cast<int>(rotatedRect.size.width) - 20;
        int h = static_cast<int>(rotatedRect.size.height) - 20;

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
                size_t largestPoints = 0;
                cv::Rect bb;

                for (auto& contour : contours) {
                    if (contour.size() > largestPoints) {
                        largestPoints = contour.size();
                        bb = cv::boundingRect(contour);

                        if (bb.width > 3 && bb.height > 3) {
                            bb.x += 3;
                            bb.y += 3;
                            bb.width -= 6;
                            bb.height -= 6;
                        }
                    }
                }

                if (bb.x >= 0 && bb.y >= 0 && bb.width > 0 && bb.height > 0
                    && bb.x + bb.width < marker.size[0] && bb.y + bb.height < marker.size[1]) {
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

                    // Find marker pattern with closest distance
                    match_result res = findMatchingMarker(thresholdedCode);

                    if (res.score > 0.5f) {
                        // Create ringbuffer to store marker position history
                        if (markersHistory.find(res.pattern) == markersHistory.end()) {
                            markersHistory[res.pattern] = ringbuffer<Point>(MARKER_HISTORY_LENGTH);
                        }

                        // Add new position
                        auto newPos = Point(brect.x + roi.x + bb.x, brect.y + roi.y + bb.y);
                        markersHistory[res.pattern].add(newPos);

                        // Calculate moving average to determine filtered current position
                        Point movingPos = averageOfPoints(markersHistory[res.pattern].data());

                        // Determine rotation
                        double rotation = (rotatedRect.angle - (int) res.rotation);

                        if (rotation < 0) {
                            rotation += 360.0;
                        }

                        markers.push_back(detected_marker(res.pattern, newPos, rotation));
                    }
                }
            }
        }
    }

    return markers;
}

match_result detector::findMatchingMarker(const Mat& detectedPattern) const {
    // Calculate all permutations of input pattern
    vector<std::pair<exact_angle, Mat>> inputPermutations = {
        std::make_pair(CLOCKWISE_0, detectedPattern),
        std::make_pair(CLOCKWISE_90, rotateExact(detectedPattern, CLOCKWISE_90)),
        std::make_pair(CLOCKWISE_180, rotateExact(detectedPattern, CLOCKWISE_180)),
        std::make_pair(CLOCKWISE_270, rotateExact(detectedPattern, CLOCKWISE_270))
    };

    match_result bestResult;

    // Find the marker pattern that best matches any rotation of the detected pattern
    for (size_t markerId = 0; markerId < markerPatterns.size(); markerId++) {
        const Mat& marker = markerPatterns[markerId];

        for (auto& permutation : inputPermutations) {
            double score = cv::countNonZero(marker == permutation.second) / 36.0;

            if (score > bestResult.score) {
                bestResult = match_result(markerId, score, permutation.first);
            }
        }
    }

    return bestResult;
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

void detector::stop() {
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

Mat detector::rotateExact(Mat src, exact_angle angle) {
    Mat tmp, result;

    switch (angle) {
    case CLOCKWISE_90:
        cv::transpose(src, tmp);
        cv::flip(tmp, result, 1);
        break;

    case CLOCKWISE_180:
        cv::flip(src, result, -1);
        break;

    case CLOCKWISE_270:
        cv::transpose(src, tmp);
        cv::flip(tmp, result, 0);
        break;

    default:
        result = src;
    }

    return result;
}

}
