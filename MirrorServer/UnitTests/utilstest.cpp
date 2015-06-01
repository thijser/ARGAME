#include <gtest/gtest.h>
#include <opencv2/highgui/highgui.hpp>
#include "cvutils.hpp"

using namespace mirrors;
using namespace cv;

TEST(UtilsTest, Rotate) {
    Mat frame0 = imread("tests/rotationtest_nonexact_0.png");
    Mat frame90 = imread("tests/rotationtest_nonexact_90.png");

    ASSERT_EQ(cv::countNonZero(frame90 == rotateImage(frame0, 90)), 64 * 64);
}

TEST(UtilsTest, RotateExactly) {
    Mat frame0 = imread("tests/rotationtest_0.png");
    Mat frame90 = imread("tests/rotationtest_90.png");
    Mat frame180 = imread("tests/rotationtest_180.png");
    Mat frame270 = imread("tests/rotationtest_270.png");

    ASSERT_EQ(cv::countNonZero(frame0 == rotateExactly(frame0, CLOCKWISE_0)), 6);
    ASSERT_EQ(cv::countNonZero(frame90 == rotateExactly(frame0, CLOCKWISE_90)), 6);
    ASSERT_EQ(cv::countNonZero(frame180 == rotateExactly(frame0, CLOCKWISE_180)), 6);
    ASSERT_EQ(cv::countNonZero(frame270 == rotateExactly(frame0, CLOCKWISE_270)), 6);
}

TEST(UtilsTest, Pivot) {
    vector<Point> contour = {
        Point(-2, -4),
        Point(4, -4),
        Point(4, 2),
        Point(-2, 2)
    };

    ASSERT_EQ(getPivot(contour), Point(1, -1));
}

TEST(UtilsTest, DistBasic) {
    ASSERT_FLOAT_EQ(dist(Point(0, 0), Point(0, 0)), 0.0f);
    ASSERT_FLOAT_EQ(dist(Point(0, 0), Point(1, 0)), 1.0f);
    ASSERT_FLOAT_EQ(dist(Point(0, 0), Point(0, 1)), 1.0f);
    ASSERT_FLOAT_EQ(dist(Point(0, 0), Point(1, 1)), sqrtf(2));
    ASSERT_FLOAT_EQ(dist(Point(2, 3), Point(6, 5)), sqrtf(20));
    ASSERT_FLOAT_EQ(dist(Point(6, 5), Point(2, 3)), sqrtf(20));
}

TEST(UtilsTest, DistNegativeCoords) {
    ASSERT_FLOAT_EQ(dist(Point(-1, -1), Point(0, 0)), sqrtf(2));
}