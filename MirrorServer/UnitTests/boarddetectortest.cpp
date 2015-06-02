#include <gtest/gtest.h>
#include <opencv2/highgui/highgui.hpp>
#include "boarddetector.hpp"
#include "testutilities.hpp"

using namespace mirrors;
using namespace cv;

TEST(BoardDetectorTest, BoardWithMarkers) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/boardtest_markers.jpg");

    EXPECT_TRUE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    EXPECT_EQ(board.cols, 570);
    EXPECT_EQ(board.rows, 720);

    Mat expected = imread("UnitTests/boardtest_markers_result.jpg");

    EXPECT_TRUE(imagesApproximatelyEqual(board, expected));
}

TEST(BoardDetectorTest, BoardWithoutMarkers) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/boardtest_no_markers.jpg");

    EXPECT_TRUE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    EXPECT_EQ(board.cols, 570);
    EXPECT_EQ(board.rows, 720);

    Mat expected = imread("UnitTests/boardtest_no_markers_result.jpg");

    EXPECT_TRUE(imagesApproximatelyEqual(board, expected));
}

TEST(BoardDetectorTest, BoardWithoutLocate) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/boardtest_no_markers.jpg");

    Mat board = detector.extractBoard(frame);

    EXPECT_EQ(board.cols, 0);
    EXPECT_EQ(board.rows, 0);
}

TEST(BoardDetectorTest, NoBoard) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/boardtest_no_board.jpg");

    EXPECT_FALSE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    EXPECT_EQ(board.cols, 0);
    EXPECT_EQ(board.rows, 0);
}

TEST(BoardDetectorTest, PartialBoard) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/boardtest_partial.jpg");

    EXPECT_FALSE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    EXPECT_EQ(board.cols, 0);
    EXPECT_EQ(board.rows, 0);
}

TEST(BoardDetectorTest, RedChair) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/boardtest_red_chair.jpg");

    EXPECT_FALSE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    EXPECT_EQ(board.cols, 0);
    EXPECT_EQ(board.rows, 0);
}
