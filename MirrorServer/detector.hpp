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

struct marker_locations {
    vector<vector<Point>> contours;
    vector<Vec4i> hierarchy;
    vector<size_t> candidates;
};

class detector {
public:
    detector(int captureDevice = 0, int requestedWidth = 1600, int requestedHeight = 896);

    void detect(const detection_callback& callback);
    void loop(const detection_callback& callback);

    void stop() throw();

private:
    VideoCapture cap;
    int width, height;
    bool keepGoing;

    vector<vector<Point2f>> cornersHistory;

    Mat capture();

    vector<Point2f> getAveragedCorners(const Mat& rawFrame);
    vector<Point2f> findCorners(const Mat& rawFrame) const;

    Mat correctPerspective(const Mat& rawFrame, const vector<Point2f>& corners) const;
    Mat thresholdGreen(const Mat& correctedFrame) const;

    marker_locations locateMarkers(const Mat& thresholdedFrame) const;
    Mat findMarker(const Mat& correctedFrame, const marker_locations& data) const;

    static Point averageOfPoints(const vector<Point>& points);
    static Mat rotate(Mat src, double angle);
};

#endif
