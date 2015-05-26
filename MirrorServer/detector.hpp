/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

#ifndef DETECTOR_HPP
#define DETECTOR_HPP

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <functional>
#include <unordered_map>
#include "ringbuffer.hpp"

// Amount of frames to average for corner positions
const int CORNER_HISTORY_LENGTH = 30;

// Amount of frames to average for marker positions
const int MARKER_HISTORY_LENGTH = 5;

using cv::Mat;
using cv::Point;
using cv::Point2f;
using cv::Vec4i;
using cv::Size;
using cv::VideoCapture;

using std::vector;

struct detected_marker {
    int id;
    Point position;
    float rotation;

    detected_marker(int id, Point position, float rotation)
        : id(id), position(position), rotation(rotation) {}
};

typedef std::function<void(const Mat&, vector<detected_marker>)> detection_callback;

enum exact_angle {
    CLOCKWISE_0 = 0,
    CLOCKWISE_90 = 90,
    CLOCKWISE_180 = 180,
    CLOCKWISE_270 = 270
};

struct match_result {
    int pattern;
    double score;
    exact_angle rotation;

    match_result(int pattern = 0, double score = 0, exact_angle rotation = CLOCKWISE_0)
        : pattern(pattern), score(score), rotation(rotation) {}
};

struct marker_locations {
    vector<vector<Point>> contours;
    vector<Vec4i> hierarchy;
    vector<size_t> candidates;
};

class detector {
public:
    detector(int captureDevice = 0, int requestedWidth = 1600, int requestedHeight = 896);

    bool registerMarkers(const vector<Mat>& markers);

    void detect(const detection_callback& callback);
    void loop(const detection_callback& callback);

    void stop() throw();

private:
    VideoCapture cap;
    int width, height;
    bool keepGoing;

    // Marker 6x6 patterns
    vector<Mat> markerPatterns;

    // History of corner positions
    ringbuffer<vector<Point2f>> cornersHistory;

    // History of marker positions
    std::unordered_map<int, ringbuffer<Point>> markersHistory;

    Mat capture();

    vector<Point2f> getAveragedCorners(const Mat& rawFrame);
    vector<Point2f> findCorners(const Mat& rawFrame) const;

    Mat correctPerspective(const Mat& rawFrame, const vector<Point2f>& corners) const;
    Mat thresholdGreen(const Mat& correctedFrame) const;

    marker_locations locateMarkers(const Mat& thresholdedFrame) const;
    vector<detected_marker> recognizeMarkers(const Mat& correctedFrame, const marker_locations& data);

    match_result findMatchingMarker(const Mat& detectedPattern) const;

    static Point averageOfPoints(const vector<Point>& points);
    static Mat rotate(Mat src, double angle);
    static Mat rotateExact(Mat src, exact_angle angle);
};

#endif
