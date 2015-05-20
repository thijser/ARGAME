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

typedef std::function<void(const Mat&, const vector<Point2f>&)> detection_callback;

class detector {
public:
    detector(int captureDevice = 0, int requestedWidth = 1280, int requestedHeight = 720);

    void setSurfaceCorners(const std::vector<Point2f>& corners);

    void detect(const detection_callback& callback);
    void loop(const detection_callback& callback);

private:
    VideoCapture cap;
    int width, height;
    std::vector<Point2f> surfaceCorners;

    Mat capture();

    Mat correctPerspective(const Mat& rawFrame) const;
    Mat thresholdGreen(const Mat& correctedFrame) const;

    vector<Point2f> locateMarkers(const Mat& thresholdedFrame) const;
};

#endif