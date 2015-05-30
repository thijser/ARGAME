#include "cvutil.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace Mirrors {

float dist(const Point& a, const Point& b) {
    int dx = a.x - b.x;
    int dy = a.y - b.y;
    return static_cast<float>(std::sqrt(dx * dx + dy * dy));
}

Point boundingCenter(const vector<Point>& contour) {
    auto rect = cv::boundingRect(contour);
    return Point(rect.x + rect.width / 2, rect.y + rect.height / 2);
}

float average(const vector<float>& vals) {
    float sum = 0;

    for (float n : vals) {
        sum += n;
    }

    return sum / vals.size();
}

Point average(const vector<Point>& vals) {
    Point sum;

    for (Point n : vals) {
        sum += n;
    }

    return Point(sum.x / vals.size(), sum.y / vals.size());
}

// Source: http://opencv-code.com/quick-tips/how-to-rotate-image-in-opencv/
Mat rotate(Mat src, float angle) {
    int len = std::max(src.cols, src.rows);
    cv::Point2f pt(len / 2.f, len / 2.f);
    cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);

    Mat dst;
    cv::warpAffine(src, dst, r, cv::Size(len, len));

    return dst;
}

Mat rotateExact(Mat src, ExactAngle angle) {
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