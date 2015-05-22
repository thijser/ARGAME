#ifndef DETECTOR_HPP
#define DETECTOR_HPP

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <functional>

// Amount of frames to average for corner positions
const int cornerMovingAverageHistory = 30;

using cv::Mat;
using cv::Point;
using cv::Point2f;
using cv::Vec4i;
using cv::Size;
using cv::VideoCapture;

using std::vector;

typedef std::function<void(const Mat&, vector<Point>)> detection_callback;

struct match_result {
    int pattern;
    double score;

    match_result(int pattern = 0, double score = 0)
        : pattern(pattern), score(score) {}
};

struct marker_locations {
    vector<vector<Point>> contours;
    vector<Vec4i> hierarchy;
    vector<size_t> candidates;
};

enum exact_angle {
    CLOCKWISE_90,
    CLOCKWISE_180,
    CLOCKWISE_270
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
    vector<vector<Point2f>> cornersHistory;

    Mat capture();

    vector<Point2f> getAveragedCorners(const Mat& rawFrame);
    vector<Point2f> findCorners(const Mat& rawFrame) const;

    Mat correctPerspective(const Mat& rawFrame, const vector<Point2f>& corners) const;
    Mat thresholdGreen(const Mat& correctedFrame) const;

    marker_locations locateMarkers(const Mat& thresholdedFrame) const;
    Mat findMarker(const Mat& correctedFrame, const marker_locations& data) const;

    match_result findMatchingMarker(const Mat& detectedPattern) const;

    static Point averageOfPoints(const vector<Point>& points);
    static Mat rotate(Mat src, double angle);
    static Mat rotateExact(Mat src, exact_angle angle);
};

#endif
