#include <gtest/gtest.h>
#include <opencv2/highgui/highgui.hpp>
#include "cvutils.hpp"
#include "testutilities.hpp"

using namespace mirrors;
using namespace cv;

TEST(UtilsTest, Rotate) {
    Mat frame0 = imread("UnitTests/rotationtest_nonexact_0.png");
    Mat expected = imread("UnitTests/rotationtest_nonexact_90.png");
    Mat rotated = rotateImage(frame0, 90);

    ASSERT_EQ(countNonZeroMultichannel(rotated == expected), expected.rows * expected.cols);
}

TEST(UtilsTest, RotateCrop) {
    Mat marker = imread("UnitTests/rotationtest_non_square.png");
    Mat expected = imread("UnitTests/rotationtest_non_square_10.png");
    Mat rotated = rotateImage(marker, 10);

    ASSERT_EQ(countNonZeroMultichannel(rotated == expected), expected.cols * expected.rows);
}

TEST(UtilsTest, RotateExactly) {
    Mat frame0 = imread("UnitTests/rotationtest_0.png");
    Mat frame90 = imread("UnitTests/rotationtest_90.png");
    Mat frame180 = imread("UnitTests/rotationtest_180.png");
    Mat frame270 = imread("UnitTests/rotationtest_270.png");

    ASSERT_EQ(countNonZeroMultichannel(frame0 == rotateExactly(frame0, CLOCKWISE_0)), 6);
    ASSERT_EQ(countNonZeroMultichannel(frame90 == rotateExactly(frame0, CLOCKWISE_90)), 6);
    ASSERT_EQ(countNonZeroMultichannel(frame180 == rotateExactly(frame0, CLOCKWISE_180)), 6);
    ASSERT_EQ(countNonZeroMultichannel(frame270 == rotateExactly(frame0, CLOCKWISE_270)), 6);
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
