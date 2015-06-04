#include <gtest/gtest.h>
#include <opencv2/highgui/highgui.hpp>
#include "boarddetector.hpp"
#include "markerdetector.hpp"
#include "markerrecognizer.hpp"
#include "testutilities.hpp"
#include "cvutils.hpp"

using namespace mirrors;
using namespace cv;

TEST(MarkerRecognizerTest, RegisterPatterns) {
    MarkerRecognizer markerRecognizer;

    ASSERT_EQ(loadPatterns(markerRecognizer, "markers/%d.png"), 12);
}

TEST(MarkerRecognizerTest, SingleMarker) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    loadPatterns(markerRecognizer, "markers/%d.png");

    Mat frame = imread("UnitTests/markertest_single.jpg");
    boardDetector.locateBoard(frame);
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);
    auto match = markerRecognizer.recognizeMarker(board, contours.at(0));

    ASSERT_EQ(match.id, 5);
    ASSERT_NEAR(match.rotation, 0, 4);
}

TEST(MarkerRecognizerTest, MultiMarker) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    loadPatterns(markerRecognizer, "markers/%d.png");

    Mat frame = imread("UnitTests/markertest_varying_lighting.jpg");
    boardDetector.locateBoard(frame);
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);
    ASSERT_EQ(contours.size(), 4ul);

    vector<pair<Point, int>> expectedMatches = {
        make_pair(Point(103, 99), 5),
        make_pair(Point(476, 93), 6),
        make_pair(Point(97, 621), 11),
        make_pair(Point(486, 627), 1)
    };

    for (auto& contour : contours) {
        Point pivot = getPivot(contour);
        auto match = markerRecognizer.recognizeMarker(board, contour);

        for (auto& expect : expectedMatches) {
            if (dist(pivot, expect.first) < 5) {
                ASSERT_EQ(expect.second, match.id);
            }
        }
    }
}

TEST(MarkerRecognizerTest, MultiMarkerGradient) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    loadPatterns(markerRecognizer, "markers/%d.png");

    Mat frame = imread("UnitTests/markertest_lighting_gradient.jpg");
    boardDetector.locateBoard(frame);
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);
    ASSERT_EQ(contours.size(), 12);

    vector<pair<Point, int>> expectedMatches = {
        make_pair(Point(130, 84), 3),
        make_pair(Point(270, 134), 10),
        make_pair(Point(112, 237), 8),
        make_pair(Point(212, 303), 2),
        make_pair(Point(310, 351), 11),
        make_pair(Point(496, 317), 5),
        make_pair(Point(100, 404), 9),
        make_pair(Point(182, 499), 0),
        make_pair(Point(286, 461), 4),
        make_pair(Point(108, 591), 7),
        make_pair(Point(320, 615), 1),
        make_pair(Point(480, 523), 6),
    };

    for (auto& contour : contours) {
        Point pivot = getPivot(contour);
        auto match = markerRecognizer.recognizeMarker(board, contour);

        for (auto& expect : expectedMatches) {
            if (dist(pivot, expect.first) < 5) {
                ASSERT_EQ(expect.second, match.id);
            }
        }
    }
}

TEST(MarkerRecognizerTest, MultiShades) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    loadPatterns(markerRecognizer, "markers/%d.png");

    Mat frame = imread("UnitTests/markertest_green_shades.jpg");
    boardDetector.locateBoard(frame);
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);
    ASSERT_EQ(contours.size(), 9);

    vector<pair<Point, int>> expectedMatches = {
        make_pair(Point(119, 183), 7),
        make_pair(Point(277, 183), 2),
        make_pair(Point(441, 183), 5),
        make_pair(Point(121, 353), 0),
        make_pair(Point(279, 354), 10),
        make_pair(Point(448, 352), 1),
        make_pair(Point(116, 525), 9),
        make_pair(Point(283, 536), 3),
        make_pair(Point(450, 537), 4)
    };

    for (auto& contour : contours) {
        Point pivot = getPivot(contour);
        auto match = markerRecognizer.recognizeMarker(board, contour);

        for (auto& expect : expectedMatches) {
            if (dist(pivot, expect.first) < 5) {
                ASSERT_EQ(expect.second, match.id);
            }
        }
    }
}

TEST(MarkerRecognizerTest, MultiSkewed) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    loadPatterns(markerRecognizer, "markers/%d.png");

    Mat frame = imread("UnitTests/boardtest_markers.jpg");
    boardDetector.locateBoard(frame);
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);
    ASSERT_EQ(contours.size(), 11);

    vector<pair<Point, int>> expectedMatches = {
        make_pair(Point(102, 101), 7),
        make_pair(Point(243, 96), 6),
        make_pair(Point(371, 140), 2),
        make_pair(Point(192, 195), 3),
        make_pair(Point(80, 320), 5),
        make_pair(Point(249, 306), 4),
        make_pair(Point(409, 269), 10),
        make_pair(Point(95, 518), 8),
        make_pair(Point(259, 470), 1),
        make_pair(Point(465, 434), 0),
        make_pair(Point(462, 646), 9)
    };

    for (auto& contour : contours) {
        Point pivot = getPivot(contour);
        auto match = markerRecognizer.recognizeMarker(board, contour);

        for (auto& expect : expectedMatches) {
            if (dist(pivot, expect.first) < 5) {
                ASSERT_EQ(expect.second, match.id);
            }
        }
    }
}

TEST(MarkerRecognizerTest, MultiTable) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    loadPatterns(markerRecognizer, "markers/%d.png");

    Mat frame = imread("UnitTests/boardtest_table.jpg");
    boardDetector.locateBoard(frame);
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);
    ASSERT_EQ(contours.size(), 11);

    vector<pair<Point, int>> expectedMatches = {
        make_pair(Point(170, 275), 7),
        make_pair(Point(296, 281), 11),
        make_pair(Point(433, 308), 5),
        make_pair(Point(42, 390), 8),
        make_pair(Point(164, 429), 1),
        make_pair(Point(279, 407), 0),
        make_pair(Point(454, 421), 3),
        make_pair(Point(59, 530), 9),
        make_pair(Point(166, 605), 2),
        make_pair(Point(343, 541), 10),
        make_pair(Point(476, 531), 4)
    };

    for (auto& contour : contours) {
        Point pivot = getPivot(contour);
        auto match = markerRecognizer.recognizeMarker(board, contour);

        for (auto& expect : expectedMatches) {
            if (dist(pivot, expect.first) < 5) {
                ASSERT_EQ(expect.second, match.id);
            }
        }
    }
}

TEST(MarkerRecognizerTest, LowRes) {
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    loadPatterns(markerRecognizer, "markers/%d.png");

    Mat frame = imread("UnitTests/boardtest_table_small.jpg");
    boardDetector.locateBoard(frame);
    Mat board = boardDetector.extractBoard(frame);

    auto contours = markerDetector.locateMarkers(board);
    ASSERT_EQ(contours.size(), 11);

    vector<pair<Point, int>> expectedMatches = {
        make_pair(Point(170, 275), 7),
        make_pair(Point(296, 281), 11),
        make_pair(Point(433, 308), 5),
        make_pair(Point(42, 390), 8),
        make_pair(Point(164, 429), 1),
        make_pair(Point(279, 407), 0),
        make_pair(Point(454, 421), 3),
        make_pair(Point(59, 530), 9),
        make_pair(Point(166, 605), 2),
        make_pair(Point(343, 541), 10),
        make_pair(Point(476, 531), 4)
    };

    for (auto& contour : contours) {
        Point pivot = getPivot(contour);
        auto match = markerRecognizer.recognizeMarker(board, contour);

        for (auto& expect : expectedMatches) {
            if (dist(pivot, expect.first) < 5) {
                ASSERT_EQ(expect.second, match.id);
            }
        }
    }
}
