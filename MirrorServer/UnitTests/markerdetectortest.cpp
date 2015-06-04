#include <gtest/gtest.h>
#include <opencv2/highgui/highgui.hpp>
#include "boarddetector.hpp"
#include "markerdetector.hpp"
#include "testutilities.hpp"
#include "cvutils.hpp"
#include <vector>

using namespace mirrors;
using namespace cv;

TEST(MarkerDetectorTest, NoMarkers) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    Mat frame = imread("UnitTests/boardtest_no_markers.jpg");

    ASSERT_TRUE(boardDetector.locateBoard(frame));
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);

    ASSERT_EQ(contours.size(), 0ul);
}

TEST(MarkerDetectorTest, SingleMarker) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    Mat frame = imread("UnitTests/markertest_single.jpg");

    ASSERT_TRUE(boardDetector.locateBoard(frame));
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);

    ASSERT_EQ(contours.size(), 1ul);

    Point pos = getPivot(contours[0]);

    ASSERT_NEAR(dist(pos, Point(248, 334)), 0, 5);
}

TEST(MarkerDetectorTest, MarkerGrid) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    Mat frame = imread("UnitTests/markertest_grid.jpg");

    ASSERT_TRUE(boardDetector.locateBoard(frame));
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);

    ASSERT_EQ(contours.size(), 9ul);

    vector<Point> expectedPivots = {
        Point(115, 181),
        Point(276, 183),
        Point(446, 182),
        Point(120, 353),
        Point(278, 355),
        Point(447, 354),
        Point(116, 525),
        Point(279, 534),
        Point(449, 534)
    };

    ASSERT_TRUE(expectMarkers(expectedPivots, contours));
}

TEST(MarkerDetectorTest, MarkerShades) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    Mat frame = imread("UnitTests/markertest_green_shades.jpg");

    ASSERT_TRUE(boardDetector.locateBoard(frame));
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);

    ASSERT_EQ(contours.size(), 9ul);

    vector<Point> expectedPivots = {
        Point(119, 183),
        Point(277, 183),
        Point(440, 183),
        Point(121, 352),
        Point(279, 355),
        Point(448, 354),
        Point(116, 525),
        Point(283, 537),
        Point(451, 537)
    };

    ASSERT_TRUE(expectMarkers(expectedPivots, contours));
}
\
TEST(MarkerDetectorTest, VaryingLighting) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    Mat frame = imread("UnitTests/markertest_varying_lighting.jpg");

    ASSERT_TRUE(boardDetector.locateBoard(frame));
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);

    ASSERT_EQ(contours.size(), 4ul);

    vector<Point> expectedPivots = {
        Point(103, 99),
        Point(476, 93),
        Point(97, 621),
        Point(486, 627)
    };

    ASSERT_TRUE(expectMarkers(expectedPivots, contours));
}

TEST(MarkerDetectorTest, LightingGradient) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    Mat frame = imread("UnitTests/markertest_lighting_gradient.jpg");

    ASSERT_TRUE(boardDetector.locateBoard(frame));
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);

    ASSERT_EQ(contours.size(), 12ul);

    vector<Point> expectedPivots = {
        Point(130, 85),
        Point(270, 135),
        Point(111, 238),
        Point(211, 304),
        Point(310, 351),
        Point(496, 317),
        Point(100, 404),
        Point(286, 460),
        Point(183, 499),
        Point(480, 523),
        Point(108, 591),
        Point(320, 616)
    };

    ASSERT_TRUE(expectMarkers(expectedPivots, contours));
}

TEST(MarkerDetectorTest, Table) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    Mat frame = imread("UnitTests/boardtest_table.jpg");

    ASSERT_TRUE(boardDetector.locateBoard(frame));
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);

    ASSERT_EQ(contours.size(), 11ul);

    vector<Point> expectedPivots = {
        Point(170, 275),
        Point(296, 281),
        Point(433, 308),
        Point(42, 390),
        Point(164, 429),
        Point(279, 407),
        Point(454, 421),
        Point(59, 530),
        Point(166, 605),
        Point(343, 541),
        Point(476, 531)
    };

    ASSERT_TRUE(expectMarkers(expectedPivots, contours));
}
