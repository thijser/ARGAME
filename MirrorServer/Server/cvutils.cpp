#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace mirrors {
    using namespace cv;

    // Source: http://stackoverflow.com/questions/22041699/rotate-an-image-without-cropping-in-opencv-in-c
    Mat rotateImage(Mat src, float angle) {
        Point2f center(src.cols / 2.0, src.rows / 2.0);
        Mat rot = getRotationMatrix2D(center, -angle, 1.0);

        Rect bbox = RotatedRect(center, src.size(), angle).boundingRect();

        rot.at<double>(0, 2) += bbox.width / 2.0 - center.x;
        rot.at<double>(1, 2) += bbox.height / 2.0 - center.y;

        Mat dst;
        warpAffine(src, dst, rot, bbox.size());

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
