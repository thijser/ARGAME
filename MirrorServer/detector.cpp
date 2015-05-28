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
    : keepGoing(true) {
    cap.open(captureDevice);

    cap.set(CV_CAP_PROP_FRAME_WIDTH, requestedWidth);
    cap.set(CV_CAP_PROP_FRAME_HEIGHT, requestedHeight);

    // Get actual resolution
    width = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_WIDTH));
    height = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_HEIGHT));

    markerScales = ringbuffer<double>(MARKER_SCALE_HISTORY_LENGTH);
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

    auto corners = getCorners(frame);

    if (corners.size() == 4) {
        Mat correctedFrame = correctPerspective(frame, corners);
        Mat thresholdedFrame = thresholdGreen(correctedFrame);

        // Use thresholded image to locate marker candidates
        auto data = locateMarkers(thresholdedFrame);

        // Track markers
        auto markers = trackMarkers(correctedFrame, data);

        callback(correctedFrame, markers);
    } else {
        callback(frame, vector<detected_marker>());
    }
}

vector<Point2f> detector::getCorners(const Mat& rawFrame) {
    if (boardCorners.size() == 4) {
        return boardCorners;
    } else {
        boardCorners = findCorners(rawFrame);
        return boardCorners;
    }
}

vector<Point2f> detector::findCorners(const Mat& rawFrame) const {
    // Give camera time to warm up before starting detection
    if (clock() - startTime < CLOCKS_PER_SEC) return vector<Point2f>();

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
        markerPoints.push_back(boundingCenter(points));
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

vector<detected_marker> detector::trackMarkers(const Mat& correctedFrame, const marker_locations& data) {
    // Start by marking everything as not seen
    size_t unseenMarkerCount = markerStates.size();
    size_t newMarkerCount = 0;

    for (auto& state : markerStates) {
        state.updatedThisFrame = false;
        state.newThisFrame = false;
    }

    for (size_t contourId : data.candidates) {
        auto& contour = data.contours[contourId];

        // Determine position of marker
        Point center = boundingCenter(contour);

        // Find closest previously seen marker
        marker_state* closestMarker = nullptr;

        for (auto& state : markerStates) {
            if (closestMarker == nullptr || dist(center, state.pos) < dist(center, closestMarker->pos)) {
                closestMarker = &state;
            }
        }

        // If there is one within a certain distance, assume it's the same marker
        if (closestMarker != nullptr && dist(center, closestMarker->pos) <= MARKER_MAX_FRAME_DIST) {
            closestMarker->updatedThisFrame = true;
            unseenMarkerCount--;

            closestMarker->velocity = dist(closestMarker->pos, center);

            closestMarker->pos = center;
            closestMarker->lastSighting = clock();
            
            auto newRecognition = recognizeMarker(correctedFrame, contour);

            // If the same pattern is still detected, update the recognized rotation
            if (newRecognition.id == closestMarker->recognition_state.id) {
                double newRot = newRecognition.rotation;
                closestMarker->rotations.add(newRot);

                double movingRot = average(closestMarker->rotations.data());

                // If current angle is significantly different than average, then discard previous angles
                // The second case here is for comparing angles like 359 and 0
                double angDiff = std::min(std::abs(movingRot - newRot), std::abs(movingRot - newRot - 360));

                if (angDiff > 3) {
                    for (int i = 0; i < MARKER_HISTORY_LENGTH; i++) {
                        closestMarker->rotations.add(newRot);
                    }
                    movingRot = newRot;
                }
                
                closestMarker->rotation = movingRot;

                // Also add the scale to average it across markers and time
                markerScales.add(newRecognition.scale);
            }

            // Only use new recognition to update state if motion blur influence is low.
            if (closestMarker->velocity <= MARKER_MAX_RECOGNITION_VELOCITY) {
                // If the new recognition has a higher confidence, replace the old one with it
                if (newRecognition.confidence >= closestMarker->recognition_state.confidence) {
                    closestMarker->recognition_state = newRecognition;
                }
            }
        } else {
            // Create initial marker state
            marker_state newMarker;

            newMarker.id = nextId++;
            newMarker.pos = center;

            markerStates.push_back(newMarker);

            newMarkerCount++;
        }
    }

    // If exactly 1 marker was not seen and exactly 1 new marker has appeared,
    // then assume that the marker has moved very quickly.
    if (unseenMarkerCount == 1 && newMarkerCount == 1) {
        for (auto& state : markerStates) {
            if (!state.updatedThisFrame && !state.newThisFrame) {
                state.pos = markerStates[markerStates.size() - 1].pos;
                state.lastSighting = clock();
                break;
            }
        }

        markerStates.pop_back();
    }

    // Clean up markers that haven't been seen in a while (500 ms)
    clock_t now = clock();
    bool done = false;

    while (!done) {
        done = true;

        for (size_t i = 0; i < markerStates.size(); i++) {
            if (now - markerStates[i].lastSighting > CLOCKS_PER_SEC / 2) {
                markerStates.erase(markerStates.begin() + i);
                done = false;
                break;
            }
        }
    }

    // Build list of updated marker data
    vector<detected_marker> detectedMarkers;
    double markerScale = average(markerScales.data());

    for (auto& marker : markerStates) {
        if (marker.recognition_state.id != -1) {
            // Convert marker coordinates to scaled coordinates
            auto p = Point2f(marker.pos);
            p /= markerScale;

            detectedMarkers.push_back(detected_marker(marker.recognition_state.id, p, static_cast<float>(marker.rotation)));
        }
    }

    return detectedMarkers;
}

recognition_result detector::recognizeMarker(const Mat& correctedFrame, const vector<Point>& contour) const {
    auto brect = cv::boundingRect(contour);

    cv::RotatedRect rotatedRect = cv::minAreaRect(contour);

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

        // Threshold for non-green area to find black/white region
        Mat markerParts[3];
        split(marker, markerParts);

        Mat mask;
        if (approach == SEGMENTATION_FAINT_GREEN) {
            mask = ~(markerParts[1] > 50 & (markerParts[1] > markerParts[2]) & (markerParts[1] > markerParts[0]));

            Mat maskClean;
            auto kernel = getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
            erode(mask, maskClean, kernel);
            dilate(maskClean, mask, kernel);
        } else {
            mask = ~(markerParts[1] > 50 & (markerParts[1] > markerParts[2]) & (markerParts[1] > markerParts[0] * 1.2));
        }

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
                    // Add new rotation
                    double newRot = (rotatedRect.angle - (int) res.rotation);

                    if (newRot < 0) {
                        newRot += 360.0;
                    }

                    // Determine scale of square marker
                    double scale = (rotatedRect.size.width + rotatedRect.size.height) / 2;

                    return recognition_result(res.pattern, res.score, newRot, scale);
                }
            }
        }
    }

    // No pattern recognised
    return recognition_result(-1, 0, 0);
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
    static Point2f dst[] = {
        Point(0, 0),
        Point(width, 0),
        Point(0, height),
        Point(width, height)
    };

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

    Mat rawThreshold, kernel;
    if (approach == SEGMENTATION_FAINT_GREEN) {
        rawThreshold = greenThreshold & (frameParts[1] > frameParts[2]) & (frameParts[1] > frameParts[0]);
        kernel = getStructuringElement(cv::MORPH_RECT, cv::Size(10, 10));
    } else {
        rawThreshold = greenThreshold & (frameParts[1] > frameParts[2]) & (frameParts[1] > frameParts[0] * 1.2);
        kernel = getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    }

    // Clean up thresholded image by eroding noise and dilating to remove holes
    Mat tmp, cleanThreshold;
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

double detector::dist(const Point& a, const Point& b) {
    double dx = a.x - b.x;
    double dy = a.y - b.y;
    return std::sqrt(dx * dx + dy * dy);
}

Point detector::boundingCenter(const vector<Point>& contour) {
    auto rect = cv::boundingRect(contour);
    return Point(rect.x + rect.width / 2, rect.y + rect.height / 2);
}

double detector::average(const vector<double>& vals) {
    double sum = 0;

    for (double n : vals) {
        sum += n;
    }

    return sum / vals.size();
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
