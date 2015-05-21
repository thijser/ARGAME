#ifndef DETECTOR_HPP
#define DETECTOR_HPP

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <functional>

using cv::Mat;
using cv::Point;
using cv::Point2f;
using cv::Vec4i;
using cv::Size;
using cv::VideoCapture;

using std::vector;

typedef std::function<void(const Mat&)> detection_callback;

struct marker_locations {
    vector<vector<Point>> contours;
    vector<Vec4i> hierarchy;
    vector<size_t> candidates;
};

class detector {
public:
    detector(int captureDevice = 0, int requestedWidth = 1280, int requestedHeight = 720);

    void setSurfaceCorners(const std::vector<Point2f>& corners);

    void detect(const detection_callback& callback);
    void loop(const detection_callback& callback);

    void stop() throw();

private:
    VideoCapture cap;
    int width, height;
    std::vector<Point2f> surfaceCorners;
    bool keepGoing;

    Mat capture();

    Mat correctPerspective(const Mat& rawFrame) const;
    Mat thresholdGreen(const Mat& correctedFrame) const;

    marker_locations locateMarkers(const Mat& thresholdedFrame) const;

    static Mat rotate(Mat src, double angle);
};

#endif
