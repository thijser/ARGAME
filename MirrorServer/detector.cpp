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

Detector::Detector(int captureDevice, int requestedWidth, int requestedHeight)
    : keepGoing(true) {
    cap.open(captureDevice);

    cap.set(CV_CAP_PROP_FRAME_WIDTH, requestedWidth);
    cap.set(CV_CAP_PROP_FRAME_HEIGHT, requestedHeight);

    // Get actual resolution
    width = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_WIDTH));
    height = static_cast<int>(cap.get(CV_CAP_PROP_FRAME_HEIGHT));
}

bool Detector::registerMarkers(const vector<Mat>& markers) {
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

vector<detected_marker> Detector::detect() {
    // Capture image from camera
    lastFrame = capture();

    auto corners = getCorners(lastFrame);

    if (corners.size() == 4) {
        Mat correctedFrame = correctPerspective(lastFrame, corners);
        Mat thresholdedFrame = thresholdGreen(correctedFrame);

        // Use thresholded image to locate marker candidates
        auto data = locateMarkers(thresholdedFrame);

        // Track markers
        auto markers = trackMarkers(correctedFrame, data);

        // Show board view in last frame
        lastFrame = correctedFrame;

        return markers;
    } else {
        return vector<detected_marker>();
    }
}

const Mat& Detector::getLastFrame() const {
    return lastFrame;
}

vector<Point> Detector::getCorners(const Mat& rawFrame) {
    // Give camera time to warm up before starting detection.
    if (boardCorners.empty() && clock() - startTime >= CLOCKS_PER_SEC) {
        boardCorners = CornerDetector(rawFrame).getCorners();
    }

    return boardCorners;
}

vector<detected_marker> Detector::trackMarkers(const Mat& correctedFrame, const vector<vector<Point>>& markerContours) {
    // Get remove messages for markers that changed pattern
    auto markerUpdates = discoverAndUpdateMarkers(correctedFrame, markerContours);

    // Remove markers that timed out
    auto markerTimeouts = checkMarkerTimeouts();
    std::copy(markerTimeouts.begin(), markerTimeouts.end(), markerUpdates.begin());

    // Add updates for current positions of markers that remain
    float markerScale = average(markerScales);

    for (auto& marker : markerStates) {
        if (marker.recognition_state.id != -1) {
            // Convert marker coordinates to scaled coordinates
            Point2f p = Point2f(
                        marker.pos.x / markerScale,
                        marker.pos.y / markerScale);

            markerUpdates.push_back(detected_marker(marker.recognition_state.id, p, static_cast<float>(marker.rotation)));
        }
    }

    return markerUpdates;
}

vector<detected_marker> Detector::checkMarkerTimeouts() {
    vector<detected_marker> markerUpdates;

    // Clean up markers that haven't been seen in a while (500 ms)
    clock_t now = clock();
    bool done = false;

    while (!done) {
        done = true;

        for (size_t i = 0; i < markerStates.size(); i++) {
            if (now - markerStates[i].lastSighting > CLOCKS_PER_SEC / 2) {
                if (markerStates[i].recognition_state.id != -1) {
                    markerUpdates.push_back(detected_marker(markerStates[i].recognition_state.id, Point2f(), 0, true));
                }

                markerStates.erase(markerStates.begin() + i);
                done = false;
                break;
            }
        }
    }

    return markerUpdates;
}

vector<detected_marker> Detector::discoverAndUpdateMarkers(const Mat& correctedFrame, const vector<vector<Point>>& markerContours) {
    vector<detected_marker> markerUpdates;

    // Start by marking everything as not seen this frame
    size_t unseenMarkerCount = markerStates.size();
    size_t newMarkerCount = 0;

    for (auto& state : markerStates) {
        state.updatedThisFrame = false;
        state.newThisFrame = false;
    }

    for (auto& contour : markerContours) {
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
            closestMarker->lastSighting = clock();

            // Calculate moving average of position
            closestMarker->positions.add(center);
            Point movingPos = average(closestMarker->positions.data());

            if (dist(movingPos, center) > 3) {
                for (int i = 0; i < MARKER_HISTORY_LENGTH; i++) {
                    closestMarker->positions.add(center);
                }
                movingPos = center;
            }

            closestMarker->pos = movingPos;

            auto newRecognition = recognizeMarker(correctedFrame, contour);

            // If the same pattern is still detected, update the recognized rotation
            if (newRecognition.id == closestMarker->recognition_state.id) {
                float newRot = newRecognition.rotation;
                closestMarker->rotations.add(newRot);

                float movingRot = average(closestMarker->rotations.data());

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
                if (markerScales.size() < MARKER_SCALE_HISTORY_LENGTH) {
                    markerScales.push_back(newRecognition.scale);
                }
            }

            // Only use new recognition to update state if motion blur influence is low.
            if (closestMarker->velocity <= MARKER_MAX_RECOGNITION_VELOCITY) {
                // If the new recognition has a higher confidence, replace the old one with it
                if (newRecognition.confidence >= closestMarker->recognition_state.confidence) {
                    // Register old marker as disappeared
                    if (closestMarker->recognition_state.id != -1 && newRecognition.id != closestMarker->recognition_state.id) {
                        markerUpdates.push_back(detected_marker(closestMarker->recognition_state.id, Point2f(), 0, true));
                    }

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

    return markerUpdates;
}

recognition_result Detector::recognizeMarker(const Mat& correctedFrame, const vector<Point>& contour) const {
    auto brect = cv::boundingRect(contour);

    cv::RotatedRect rotatedRect = cv::minAreaRect(contour);

    Mat marker = correctedFrame(brect);
    marker = rotate(marker, rotatedRect.angle);

    // Cut off border
    int w = static_cast<int>(rotatedRect.size.width) * 3 / 4;
    int h = static_cast<int>(rotatedRect.size.height) * 3 / 4;

    if (w > 7 && h > 7 && w <= marker.size[0] && h <= marker.size[1]) {
        cv::Rect roi;
        roi.x = marker.size[0] / 2 - w / 2;
        roi.y = marker.size[1] / 2 - h / 2;
        roi.width = w;
        roi.height = h;
        marker = marker(roi);

        // Threshold for non-green area to find black/white region
        Mat mask = ~thresholdGreen(marker);

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

                // Turn into grayscale and downsize to 8x8 pixels
                Mat codeImageGray, thresholdedCode, downsizedCode;
                cv::cvtColor(codeImage, codeImageGray, CV_BGR2GRAY);
                cv::resize(codeImageGray, downsizedCode, Size(8, 8), 0, 0, cv::INTER_LINEAR);
                
                // Threshold using average pixel brightness
                int avgPixel = 0;

                for (int i = 0; i < 8; i++) {
                    for (int j = 0; j < 8; j++) {
                        avgPixel += downsizedCode.at<unsigned char>(i, j);
                    }
                }

                avgPixel /= 64;

                cv::threshold(downsizedCode, thresholdedCode, avgPixel, 255, 0);

                // Crop to 6x6 pattern
                cv::Rect patternRegion(1, 1, 6, 6);
                thresholdedCode = thresholdedCode(patternRegion);

                // Find marker pattern with closest distance
                match_result res = findMatchingMarker(thresholdedCode);

                if (res.score > 0.75f) {
                    // Add new rotation
                    float newRot = (rotatedRect.angle - (int) res.rotation);

                    if (newRot < 0) {
                        newRot += 360.0f;
                    }

                    // Determine scale of square marker
                    float scale = (rotatedRect.size.width + rotatedRect.size.height) / 2;

                    return recognition_result(res.pattern, res.score, newRot, scale);
                }
            }
        }
    }

    // No pattern recognised
    return recognition_result(-1, 0, 0);
}

match_result Detector::findMatchingMarker(const Mat& detectedPattern) const {
    // Calculate all permutations of input pattern
    vector<std::pair<ExactAngle, Mat>> inputPermutations = {
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
            float score = cv::countNonZero(marker == permutation.second) / 36.0f;

            if (score > bestResult.score) {
                bestResult = match_result(markerId, score, permutation.first);
            }
        }
    }

    return bestResult;
}

void Detector::loop(const detection_callback& callback) {
    while (keepGoing) {
        callback(detect());
        int keyCode = cv::waitKey(10);
        if (keyCode == 27 || keyCode == 113) {
            stop();
        }
    }
    std::clog << "Detector stoppped" << std::endl;
}

void Detector::stop() {
    keepGoing = false;
}

Mat Detector::capture() {
    Mat frame;
    cap.read(frame);
    return frame;
}

Mat Detector::correctPerspective(const Mat& rawFrame, const vector<Point>& corners) const {
    static Point2f dst[] = {
        Point(0, 0),
        Point(width, 0),
        Point(0, height),
        Point(width, height)
    };

    Point2f src[4];
    for (int i = 0; i < 4; i++) {
        src[i] = corners[i];
    }

    Mat m = getPerspectiveTransform(src, dst);

    Mat tmp, output;
    warpPerspective(rawFrame, tmp, m, cv::Size(width, height));
    resize(tmp, output, cv::Size(19 * 30, 24 * 30));

    return output;
}

Mat Detector::thresholdGreen(const Mat& correctedFrame) const {
    // Convert image to HSV channels
    Mat frame_hsv;
    cvtColor(correctedFrame, frame_hsv, CV_BGR2HSV);

    // Split image into H, S, V components
    Mat channels[3];
    split(frame_hsv, channels);

    // Threshold on greenish hue, reasonable lightness and at least some saturation
    Mat mask = channels[0] > 35 & channels[0] < 90 & channels[2] > 40 & channels[2] < 200 & channels[1] > 30;

    // Clean up mask (remove noise, close holes)
    Mat kernel = getStructuringElement(cv::MORPH_RECT, Size(5, 5));
    Mat kernel2 = getStructuringElement(cv::MORPH_RECT, Size(10, 10));
    Mat mask_clean;
    cv::morphologyEx(mask, mask_clean, cv::MORPH_OPEN, kernel);
    cv::morphologyEx(mask_clean, mask, cv::MORPH_CLOSE, kernel);

    return mask;
}

vector<vector<Point>> Detector::locateMarkers(const Mat& thresholdedFrame) const {
    vector<vector<Point>> contours;
    vector<Vec4i> hierarchy;

    // findContours mutates the input, so make a copy
    findContours(Mat(thresholdedFrame), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));

    // Find contours that look like markers (outer contour with exactly one inner contour = child)
    vector<vector<Point>> potentialMarkers;

    for (size_t i = 0; i < hierarchy.size(); i++) {
        if (hierarchy[i][hierarchy_members::PARENT] < 0) {
            size_t children = 0;

            for (size_t j = 0; j < hierarchy.size(); j++) {
                if (hierarchy[j][hierarchy_members::PARENT] == static_cast<int>(i)) {
                    children++;
                }
            }

            if (children == 1) {
                potentialMarkers.push_back(contours[i]);
            }
        }
    }

    return potentialMarkers;
}

}
