#include <gtest/gtest.h>
#include <opencv2/highgui/highgui.hpp>
#include "boarddetector.hpp"
#include "testutilities.hpp"

using namespace mirrors;
using namespace cv;

TEST(BoardDetectorTest, BoardWithMarkers) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/Resources/boardtest_markers.jpg");

    ASSERT_TRUE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 693);
    ASSERT_EQ(board.rows, 896);

    Size boardSize = detector.getBoardSize();

    ASSERT_EQ(boardSize.width, 693);
    ASSERT_EQ(boardSize.height, 896);

    Mat expected = imread("UnitTests/Resources/boardtest_markers_result.jpg");

    ASSERT_TRUE(imagesApproximatelyEqual(board, expected));
}

TEST(BoardDetectorTest, BoardWithoutMarkers) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/Resources/boardtest_no_markers.jpg");

    ASSERT_TRUE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 693);
    ASSERT_EQ(board.rows, 896);

    Mat expected = imread("UnitTests/Resources/boardtest_no_markers_result.jpg");

    ASSERT_TRUE(imagesApproximatelyEqual(board, expected));
}

TEST(BoardDetectorTest, BoardWithoutLocate) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/Resources/boardtest_no_markers.jpg");

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 0);
    ASSERT_EQ(board.rows, 0);
}

TEST(BoardDetectorTest, NoBoard) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/Resources/boardtest_no_board.jpg");

    ASSERT_FALSE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 0);
    ASSERT_EQ(board.rows, 0);
}

TEST(BoardDetectorTest, PartialBoard) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/Resources/boardtest_partial.jpg");

    ASSERT_FALSE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 0);
    ASSERT_EQ(board.rows, 0);
}

TEST(BoardDetectorTest, RedChair) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/Resources/boardtest_red_chair.jpg");

    ASSERT_FALSE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 0);
    ASSERT_EQ(board.rows, 0);
}

TEST(BoardDetectorTest, BoardOnTable) {
    BoardDetector detector;
    Mat frame = imread("UnitTests/Resources/boardtest_table.jpg");

    ASSERT_TRUE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 634);
    ASSERT_EQ(board.rows, 896);

    Mat expected = imread("UnitTests/Resources/boardtest_table_result.jpg");

    ASSERT_TRUE(imagesApproximatelyEqual(board, expected));
}

TEST(BoardDetectorTest, RedYellowCornersSimple) {
    BoardDetector detector(BoardDetectionApproach::RED_YELLOW_MARKERS);
    Mat frame = imread("UnitTests/Resources/boardtest_redyellow_simple.jpg");

    ASSERT_TRUE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 470);
    ASSERT_EQ(board.rows, 600);

    Mat expected = imread("UnitTests/Resources/boardtest_redyellow_simple_result.jpg");

    ASSERT_TRUE(imagesApproximatelyEqual(board, expected));
}

TEST(BoardDetectorTest, RedYellowCornersCloth) {
    BoardDetector detector(BoardDetectionApproach::RED_YELLOW_MARKERS);
    Mat frame = imread("UnitTests/Resources/boardtest_redyellow_cloth.jpg");

    ASSERT_TRUE(detector.locateBoard(frame));

    Mat board = detector.extractBoard(frame);

    ASSERT_EQ(board.cols, 486);
    ASSERT_EQ(board.rows, 600);

    Mat expected = imread("UnitTests/Resources/boardtest_redyellow_cloth_result.jpg");

    ASSERT_TRUE(imagesApproximatelyEqual(board, expected));
}

TEST(BoardDetectorTest, RedYellowCornersHalf) {
    BoardDetector detector(BoardDetectionApproach::RED_YELLOW_MARKERS);
    Mat frame = imread("UnitTests/Resources/boardtest_redyellow_half.jpg");

    ASSERT_FALSE(detector.locateBoard(frame));
}
