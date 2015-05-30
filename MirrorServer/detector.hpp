/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file detector.hpp
* @brief Contains implementation of classes and structures
* used for camera marker detection.
*/

#ifndef DETECTOR_HPP
#define DETECTOR_HPP

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <functional>
#include <unordered_map>
#include <ctime>
#include "ringbuffer.hpp"
#include "cornerdetector.hpp"
#include "cvutil.hpp"

namespace mirrors {

/// Amount of frames to average for marker positions.
const int MARKER_HISTORY_LENGTH = 15;

/// Amount of marker scales to average.
const int MARKER_SCALE_HISTORY_LENGTH = 120;

/// Maximum distance a marker can move per frame before it's considered a new marker.
const float MARKER_MAX_FRAME_DIST = 50;

/// Maximum speed before recognition of marker pattern is disabled (pixels/frame)
const float MARKER_MAX_RECOGNITION_VELOCITY = 15;

using cv::Mat;
using cv::Point;
using cv::Point2f;
using cv::Vec4i;
using cv::Size;
using cv::VideoCapture;

using std::vector;

/**
 * @brief Information about a recognised pattern in a marker.
 */
struct recognition_result {
    /// Index of recognised pattern.
    int id;

    /// Recognition confidence score [0, 1].
    float confidence;

    /// Yaw rotation of marker.
    float rotation;

    /// Width and height of marker
    float scale;

    /**
     * @brief Creates a new structure describing a recognised pattern.
     * @param id - Index of recognised pattern or -1 if none recognised.
     * @param confidence - Confidence score of recognition (only defined for id != -1).
     * @param rotation - Rotation of marker containing pattern (only defined for id != 1).
     */
    recognition_result(int id = -1, float confidence = 0, float rotation = 0, float scale = 1)
        : id(id), confidence(confidence), rotation(rotation), scale(scale) {}
};

/**
 * @brief Information about a detected marker.
 */
struct detected_marker {
    /// Index of the recognised pattern.
    int id;

    /// X and Y position on the board
    Point2f position;

    /// Yaw rotation of marker
    float rotation;

    /// True if marker was deleted this frame (not seen for a while).
    bool deleted;

    /**
     * @brief Creates a new structure describing a detected marker.
     * @param id - Index of recognised pattern.
     * @param position - Position of marker.
     * @param rotation - Yaw rotation of marker.
     */
    detected_marker(int id, Point2f position, float rotation, bool deleted = false)
        : id(id), position(position), rotation(rotation), deleted(deleted) {
    }
};

/// Callback for detected markers in a frame.
typedef std::function<void(const vector<detected_marker>&)> detection_callback;

/**
 * @brief Result of matching input pattern with known pattern.
 */
struct match_result {
    /// Index of known pattern that input was compared against.
    int pattern;

    /// Fraction of bits that were equal (between 0 and 1).
    float score;

    /// Rotated version of known pattern that was compared.
    ExactAngle rotation;

    /**
     * @brief Creates a new structure describing an input/known pattern comparison.
     * @param pattern - Index of known pattern that was compared with input.
     * @param score - Fraction of bits that were equal.
     * @param rotation - Exact rotation of known pattern used in comparison.
     */
    match_result(int pattern = 0, float score = 0, ExactAngle rotation = CLOCKWISE_0)
        : pattern(pattern), score(score), rotation(rotation) {
    }
};

/**
 * @brief Information about detected marker.
 */
struct marker_state {
    /// Unique ID of marker.
    int id;

    /// Pattern recognised in marker.
    recognition_result recognition_state;

    /// Last known postions of marker.
    ringbuffer<Point> positions;

    /// Moving averaged position of marker.
    Point pos;

    /// Last known rotations of marker.
    ringbuffer<float> rotations;

    /// Moving averaged rotation of marker.
    float rotation;

    /// Last known velocity of marker (pixels/frame).
    float velocity;

    /// Timestamp of last known position.
    clock_t lastSighting;

    /// Flag indicating if marker has been assigned a new position this frame
    bool updatedThisFrame;

    /// Flag indicating if this marker has first appeared in this frame
    bool newThisFrame = true;

    /**
     * @brief Creates a structure describing a marker that hasn't been detected yet.
     */
    marker_state()
        : id(-1), positions(ringbuffer<Point>(MARKER_HISTORY_LENGTH)), pos(Point(-1000, -1000)),
          rotations(ringbuffer<float>(MARKER_HISTORY_LENGTH)), lastSighting(clock()) {}
};

/**
 * @brief Detector of markers from a camera image given known patterns.
 */
class Detector {
public:
    /**
     * @brief Construct a detector.
     * @param captureDevice - Index of camera device to capture images with.
     * @param requestedWidth - Desired horizontal resolution of images to capture.
     * @param requestedHeight - Desired vertical resolution of images to capture.
     */
    Detector(int captureDevice = 0, int requestedWidth = 1600, int requestedHeight = 896);

    /**
     * @brief Register new patterns that can be recognised in markers.
     * @param markers - Collection of patterns to add to set of known patterns.
     * @return Boolean indicating if added patterns are appropriate (grayscale and 6x6 pixels).
     */
    bool registerMarkers(const vector<Mat>& markers);

    /**
     * @brief Capture a single frame and detect markers in it.
     * @return Updates to markers (new markers, moved markers, removed markers).
     */
    vector<detected_marker> detect();

    /**
     * @brief Continuously capture frames and detect markers in them.
     * @param callback - Callback to pass detection results of each frame to.
     */
    void loop(const detection_callback& callback);

    /**
     * @brief Retrieve the last frame that detection ran on.
     * @return Last frame that detection ran on.
     */
    const Mat& getLastFrame() const;

    /**
     * @brief Method to abort an invocation of loop().
     */
    void stop();

private:
    /// Video capture device to capture frames from.
    VideoCapture cap;

    /// Last captured frame.
    Mat lastFrame;

    /// Actual horizontal resolution to capture frames with.
    int width;

    /// Actual vertical resolution to capture frames with.
    int height;

    /// Boolean that controls if loop() continues or stops.
    bool keepGoing;

    /// Marker 6x6 patterns.
    vector<Mat> markerPatterns;

    /// Positions of corner positions.
    vector<Point> boardCorners;

    /// Latest state of markers.
    vector<marker_state> markerStates;

    /// Average scale of markers.
    vector<float> markerScales;

    /// ID reserved for the next newly detected marker.
    int nextId = 0;

    /// Time since detection start
    clock_t startTime = clock();

    /**
     * @brief Capture a frame from the camera and return it.
     * @return New frame from the camera.
     */
    Mat capture();

    /**
     * @brief Returns found corners or searches for them.
     * @param rawFrame - Newly captured frame.
     * @return Positions of four board corners or empty collection if none were found so far.
     */
    vector<Point> getCorners(const Mat& rawFrame);

    /**
     * @brief Isolate the board from the camera view using the corner positions.
     * @param rawFrame - Image to transform.
     * @param corners - Positions of board corners in given image.
     * @return Image with isolated version of board (aspect ratio based on test board).
     */
    Mat correctPerspective(const Mat& rawFrame, const vector<Point>& corners) const;

    /**
     * @brief Isolate possible markers from the rest of the image by thresholding green.
     * @param correctedFrame - Isolated image of board from correctPerspective().
     * @return Binary image with greenish pixels.
     */
    Mat thresholdGreen(const Mat& correctedFrame) const;

    /**
     * @brief Find possible markers in a frame given a threshold on green regions.
     * @param thresholdedFrame - Binary image as returned by thresholdGreen().
     * @return Contours of possible markers.
     */
    vector<vector<Point>> locateMarkers(const Mat& thresholdedFrame) const;

    /**
    * @brief Track previously seen markers in the new frame and update their state.
    * @param correctedFrame - Image as returned by correctPerspective().
    * @param markerContours - Marker detection results from locateMarkers().
    * @return Updated marker states.
    */
    vector<detected_marker> trackMarkers(const Mat& correctedFrame, const vector<vector<Point>>& markerContours);

    /**
     * @brief Check for markers that moved, new markers and disappeared markers.
     * @param correctedFrame - Image as returned by correctPerspective().
     * @param markerContours - Marker detection results from locateMarkers().
     * @param unseenMarkerCount - Output variable for amount of unseen markers.
     * @param newMarkerCount - Output variable for amount of new markers.
     * @return Updated marker states.
     */
    vector<detected_marker> discoverAndUpdateMarkers(const Mat& correctedFrame, const vector<vector<Point>>& markerContours);

    /**
     * @brief Check and remove markers that haven't been visible for a while.
     * @return Marker updates for the deletes.
     */
    vector<detected_marker> checkMarkerTimeouts();

    /**
     * @brief Recognise the pattern of the marker described by the given contour.
     * @param correctedFrame - Image as returned by correctPerspective().
     * @param contour - Contour describing the marker in the image.
     * @return Best matching pattern, confidence and rotation of marker.
     */
    recognition_result recognizeMarker(const Mat& correctedFrame, const vector<Point>& contour) const;

    /**
     * @brief Find the pattern that best matches the given input pattern
     * (which may be rotated by n * 90 degrees).
     * @param detectedPattern - Input pattern from detected marker.
     * @return Best matching known pattern and rotation of it.
     */
    match_result findMatchingMarker(const Mat& detectedPattern) const;
};

}

#endif
