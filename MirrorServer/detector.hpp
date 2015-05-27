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

namespace mirrors {

/// Amount of frames to average for marker positions.
const int MARKER_HISTORY_LENGTH = 15;

using cv::Mat;
using cv::Point;
using cv::Point2f;
using cv::Vec4i;
using cv::Size;
using cv::VideoCapture;

using std::vector;

/**
 * @brief Information about a detected marker.
 */
struct detected_marker {
    /// Index of the recognised pattern.
    const int id;

    /// X and Y position on the board
    const Point position;

    /// Yaw rotation of marker
    const float rotation;

    /**
     * @brief Creates a new structure describing a detected marker.
     * @param id - Index of recognised pattern.
     * @param position - Position of marker.
     * @param rotation - Yaw rotation of marker.
     */
    detected_marker(int id, Point position, float rotation)
        : id(id), position(position), rotation(rotation) {
    }
};

/// Callback for detected markers in a frame.
typedef std::function<void(const Mat&, vector<detected_marker>)> detection_callback;

/**
 * @brief Angles that are multiples of 90 degrees, used for exact rotations.
 */
enum exact_angle {
    CLOCKWISE_0 = 0,
    CLOCKWISE_90 = 90,
    CLOCKWISE_180 = 180,
    CLOCKWISE_270 = 270
};

/**
 * @brief Result of matching input pattern with known pattern.
 */
struct match_result {
    /// Index of known pattern that input was compared against.
    int pattern;

    /// Fraction of bits that were equal (between 0 and 1).
    double score;

    /// Rotated version of known pattern that was compared.
    exact_angle rotation;

    /**
     * @brief Creates a new structure describing an input/known pattern comparison.
     * @param pattern - Index of known pattern that was compared with input.
     * @param score - Fraction of bits that were equal.
     * @param rotation - Exact rotation of known pattern used in comparison.
     */
    match_result(int pattern = 0, double score = 0, exact_angle rotation = CLOCKWISE_0)
        : pattern(pattern), score(score), rotation(rotation) {
    }
};

/**
 * @brief Info about possible marker objects in camera image.
 */
struct marker_locations {
    /// List of green contours that were detected.
    vector<vector<Point>> contours;

    /// Hierarchy of detected contours (outer and inner contours).
    vector<Vec4i> hierarchy;

    /// List of indices into contours collection that are likely to be markers.
    vector<size_t> candidates;
};

struct marker_state {
    int id;

    Point pos;

    clock_t lastSighting;

    marker_state() : id(-1), pos(Point(-1000, -1000)), lastSighting(clock()) {}
};

/**
 * @brief Detector of markers from a camera image given known patterns.
 */
class detector {
public:
    /**
     * @brief Construct a detector.
     * @param captureDevice - Index of camera device to capture images with.
     * @param requestedWidth - Desired horizontal resolution of images to capture.
     * @param requestedHeight - Desired vertical resolution of images to capture.
     */
    detector(int captureDevice = 0, int requestedWidth = 1600, int requestedHeight = 896);

    /**
     * @brief Register new patterns that can be recognised in markers.
     * @param markers - Collection of patterns to add to set of known patterns.
     * @return Boolean indicating if added patterns are appropriate (grayscale and 6x6 pixels).
     */
    bool registerMarkers(const vector<Mat>& markers);

    /**
     * @brief Capture a single frame and detect markers in it.
     * @param callback - Callback to pass detection results to.
     */
    void detect(const detection_callback& callback);

    /**
     * @brief Continuously capture frames and detect markers in them.
     * @param callback - Callback to pass detection results of each frame to.
     */
    void loop(const detection_callback& callback);

    /**
     * @brief Method to abort an invocation of loop().
     */
    void stop();

private:
    /// Video capture device to capture frames from.
    VideoCapture cap;

    /// Actual horizontal resolution to capture frames with.
    int width;

    /// Actual vertical resolution to capture frames with.
    int height;

    /// Boolean that controls if loop() continues or stops.
    bool keepGoing;

    /// Marker 6x6 patterns.
    vector<Mat> markerPatterns;

    /// Positions of corner positions.
    vector<Point2f> boardCorners;

    /// History of marker positions.
    std::unordered_map<int, std::pair<ringbuffer<Point>, ringbuffer<double>>> markersHistory;

    /// Latest state of markers.
    vector<marker_state> markerStates;

    /// ID reserved for the next newly detected marker.
    int nextId = 0;

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
    vector<Point2f> getCorners(const Mat& rawFrame);

    /**
     * @brief Find the positions of the four board corners in the specified frame.
     * @param rawFrame - Frame to detect corners in.
     * @return Positions of four board corners or empty collection if none were found.
     */
    vector<Point2f> findCorners(const Mat& rawFrame) const;

    /**
     * @brief Isolate the board from the camera view using the corner positions.
     * @param rawFrame - Image to transform.
     * @param corners - Positions of board corners in given image.
     * @return Image with isolated version of board (aspect ratio based on test board).
     */
    Mat correctPerspective(const Mat& rawFrame, const vector<Point2f>& corners) const;

    /**
     * @brief Isolate possible markers from the rest of the image by thresholding green.
     * @param correctedFrame - Isolated image of board from correctPerspective().
     * @return Binary image with greenish pixels.
     */
    Mat thresholdGreen(const Mat& correctedFrame) const;

    /**
     * @brief Find possible markers in a frame given a threshold on green regions.
     * @param thresholdedFrame - Binary image as returned by thresholdGreen().
     * @return Locations of possible markers.
     */
    marker_locations locateMarkers(const Mat& thresholdedFrame) const;

    /**
    * @brief Track previously seen markers in the new frame and update their state.
    * @param correctedFrame - Image as returned by correctPerspective().
    * @param data - Marker detection results from locateMarkers().
    */
    void trackMarkers(const Mat& correctedFrame, const marker_locations& data);

    /**
     * @brief Recognise the patterns of potential markers.
     * @param correctedFrame - Image as returned by correctPerspective().
     * @param data - Marker detection results from locateMarkers().
     * @return Collection of detected markers and patterns.
     */
    vector<detected_marker> recognizeMarkers(const Mat& correctedFrame, const marker_locations& data);

    /**
     * @brief Find the pattern that best matches the given input pattern
     * (which may be rotated by n * 90 degrees).
     * @param detectedPattern - Input pattern from detected marker.
     * @return Best matching known pattern and rotation of it.
     */
    match_result findMatchingMarker(const Mat& detectedPattern) const;

    /**
     * @brief Calculate average of given points.
     * @param points - Collection of points to calculate average from.
     * @return Point with average X and Y of specified points.
     */
    static Point averageOfPoints(const vector<Point>& points);

    /**
     * @brief Calculate average of given numbers.
     * @param vals - Collection of values to calculate average from.
     * @return Average value of given numbers.
     */
    static double average(const vector<double>& vals);

    /**
     * @brief Calculate Euclidean distance between two points.
     * @param a - First point.
     * @param b - Second point.
     * @return Euclidean distance between the first and second point.
     */
    static double dist(const Point& a, const Point& b);

    /**
     * @brief Calculate the center of a marker given the contour.
     * @param contour - The contour that represents the shape of the marker.
     * @return The center point of the marker depicted by the contour.
     */
    static Point markerCenter(const vector<Point>& contour);

    /**
     * @brief Rotate image by arbitrary angle.
     * @param src - Input image.
     * @param angle - Angle rotate image by in clockwise direction (may be negative for counter-clockwise).
     * @return Rotated image, which may be resized to fit the result.
     */
    static Mat rotate(Mat src, double angle);

    /**
     * @brief Rotate image by multiple of 90 degrees.
     * @param src - Input image.
     * @param angle - Multiple of 90 degrees.
     * @return Rotated image, where row/column size may be swapped.
     */
    static Mat rotateExact(Mat src, exact_angle angle);
};

}

#endif
