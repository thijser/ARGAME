#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace mirrors {

    // Source: http://opencv-code.com/quick-tips/how-to-rotate-image-in-opencv/
    Mat rotateImage(Mat src, float angle) {
        int len = std::max(src.cols, src.rows);
        cv::Point2f pt(len / 2.f, len / 2.f);
        cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);

        Mat dst;
        cv::warpAffine(src, dst, r, cv::Size(len, len));

        return dst;
    }

    Mat rotateExactly(Mat src, ExactAngle angle) {
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

    Point getPivot(const vector<Point>& contour) {
        cv::Rect rect = cv::boundingRect(contour);
        return Point(rect.x + rect.width / 2, rect.y + rect.height / 2);
    }

    float dist(const Point& a, const Point& b) {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        return (float) std::sqrt(dx * dx + dy * dy);
    }

}