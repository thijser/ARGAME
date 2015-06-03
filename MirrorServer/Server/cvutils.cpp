#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace mirrors {
    using namespace cv;

    // Source: http://stackoverflow.com/questions/22041699/rotate-an-image-without-cropping-in-opencv-in-c
    Mat rotateImage(Mat src, float angle) {
        // Determine size of image needed to contain entire rotated image
        int diagonal = (int) sqrt(src.cols*src.cols + src.rows*src.rows);
        int newWidth = diagonal;
        int newHeight = diagonal;

        int offsetX = (newWidth - src.cols) / 2;
        int offsetY = (newHeight - src.rows) / 2;

        Mat targetMat(newWidth, newHeight, src.type(), 0.0);
        Point2f src_center(targetMat.cols / 2.0f, targetMat.rows / 2.0f);

        src.copyTo(targetMat.rowRange(offsetY, offsetY + src.rows).colRange(offsetX, offsetX + src.cols));

        // Perform rotation
        Mat rot_mat = getRotationMatrix2D(src_center, -angle, 1.0);
        Mat frameRotated;
        warpAffine(targetMat, frameRotated, rot_mat, targetMat.size());

        //Calculate bounding rect for exact image
        //Reference:- http://stackoverflow.com/questions/19830477/find-the-bounding-rectangle-of-rotated-rectangle/19830964?noredirect=1#19830964
        Rect boundRect(src.cols, src.rows, 0, 0);

        int x1 = offsetX;
        int x2 = offsetX + src.cols;
        int x3 = offsetX;
        int x4 = offsetX + src.cols;

        int y1 = offsetY;
        int y2 = offsetY;
        int y3 = offsetY + src.rows;
        int y4 = offsetY + src.rows;

        Mat coordinate = (Mat_<double>(3, 4) << x1, x2, x3, x4,
            y1, y2, y3, y4,
            1, 1, 1, 1);
        Mat rotCoordinate = rot_mat * coordinate;

        for (int i = 0; i < 4; i++) {
            if (rotCoordinate.at<double>(0, i)<boundRect.x)
                boundRect.x = (int)rotCoordinate.at<double>(0, i); // access smallest
            if (rotCoordinate.at<double>(1, i)<boundRect.y)
                boundRect.y = rotCoordinate.at<double>(1, i); // access smallest y
        }

        for (int i = 0; i < 4; i++) {
            if (rotCoordinate.at<double>(0, i)>boundRect.width)
                boundRect.width = (int)rotCoordinate.at<double>(0, i); // access largest x
            if (rotCoordinate.at<double>(1, i)>boundRect.height)
                boundRect.height = rotCoordinate.at<double>(1, i); // access largest y
        }

        boundRect.width = boundRect.width - boundRect.x;
        boundRect.height = boundRect.height - boundRect.y;

        // Crop rotated image
        return frameRotated(boundRect);
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
