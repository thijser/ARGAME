#include <gtest/gtest.h>
#include <opencv2/highgui/highgui.hpp>
#include "markertracker.hpp"
#include "testutilities.hpp"
#include "cvutils.hpp"

using namespace mirrors;
using namespace cv;

class MarkerTrackerTest : public ::testing::Test {
protected:
    virtual void SetUp() {
        loadPatterns(markerRecognizer, "markers/%d.png");

        Mat frameStart = imread("UnitTests/Resources/trackertest_start.jpg");
        boardDetector.locateBoard(frameStart);
    }

    BoardDetector boardDetector = BoardDetector(BoardDetectionApproach::RED_MARKERS, false);
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;
    MarkerTracker markerTracker = MarkerTracker(boardDetector, markerDetector, markerRecognizer);

    Mat frameStart = imread("UnitTests/Resources/trackertest_start.jpg");
    Mat frameMoved = imread("UnitTests/Resources/trackertest_moved.jpg");
    Mat frameFastMoved = imread("UnitTests/Resources/trackertest_fast_move.jpg");
    Mat frameNew = imread("UnitTests/Resources/trackertest_new.jpg");
    Mat frameNewMultiple = imread("UnitTests/Resources/trackertest_new_multiple.jpg");
    Mat frameMoveMultiple = imread("UnitTests/Resources/trackertest_move_multiple.jpg");
    Mat frameRemove = imread("UnitTests/Resources/trackertest_remove.jpg");
    Mat frameOriginal = imread("UnitTests/Resources/trackertest_original.jpg");
    Mat frameUpdatedMatch = imread("UnitTests/Resources/trackertest_updated_match.jpg");
    Mat frameBackgroundMatch = imread("UnitTests/Resources/trackertest_background_pattern.jpg");
};

TEST_F(MarkerTrackerTest, NewMarker) {
    auto updates = markerTracker.track(frameStart, 0);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::NEW, 8, 225, Point(254, 432))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, MovedMarker) {
    markerTracker.track(frameStart, 0);
    auto updates = markerTracker.track(frameMoved, 33);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 8, 225, Point(255, 393))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, FastMoveMarker) {
    markerTracker.track(frameStart, 0);
    markerTracker.track(frameMoved, 33);
    auto updates = markerTracker.track(frameFastMoved, 67);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 8, 225, Point(254, 151))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, AdditionalMarker) {
    markerTracker.track(frameStart, 0);
    markerTracker.track(frameMoved, 33);
    markerTracker.track(frameFastMoved, 67);
    auto updates = markerTracker.track(frameNew, 100);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 8, 225, Point(254, 151)),
        ExpectedMarkerUpdate(MarkerUpdateType::NEW, 7, 318, Point(122, 500))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, NewMultiple) {
    markerTracker.track(frameStart, 0);
    markerTracker.track(frameMoved, 33);
    markerTracker.track(frameFastMoved, 67);
    markerTracker.track(frameNew, 100);
    auto updates = markerTracker.track(frameNewMultiple, 133);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 8, 225, Point(254, 151)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 7, 318, Point(122, 500)),

        ExpectedMarkerUpdate(MarkerUpdateType::NEW, 6, 39, Point(130, 310)),
        ExpectedMarkerUpdate(MarkerUpdateType::NEW, 9, 0, Point(423, 285)),
        ExpectedMarkerUpdate(MarkerUpdateType::NEW, 3, 140, Point(379, 504))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, MoveMultiple) {
    markerTracker.track(frameStart, 0);
    markerTracker.track(frameMoved, 33);
    markerTracker.track(frameFastMoved, 67);
    markerTracker.track(frameNew, 100);
    markerTracker.track(frameNewMultiple, 133);
    auto updates = markerTracker.track(frameMoveMultiple, 167);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 8, 215, Point(223, 112)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 7, 318, Point(152, 475)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 6, 39, Point(148, 275)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 9, 357, Point(470, 282)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 3, 144, Point(405, 555))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, Remove) {
    markerTracker.track(frameStart, 0);
    markerTracker.track(frameMoved, 33);
    markerTracker.track(frameFastMoved, 67);
    markerTracker.track(frameNew, 100);
    markerTracker.track(frameNewMultiple, 133);
    markerTracker.track(frameMoveMultiple, 167);
    auto updates = markerTracker.track(frameRemove, 200);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 8, 215, Point(216, 104)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 6, 39, Point(148, 275)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 9, 357, Point(470, 282))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, RemoveTimeout) {
    markerTracker.track(frameStart, 0);
    markerTracker.track(frameMoved, 33);
    markerTracker.track(frameFastMoved, 67);
    markerTracker.track(frameNew, 100);
    markerTracker.track(frameNewMultiple, 133);
    markerTracker.track(frameMoveMultiple, 167);
    auto updates = markerTracker.track(frameRemove, 1000);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 8, 215, Point(216, 104)),
        ExpectedMarkerUpdate(MarkerUpdateType::REMOVE, 7, 318, Point(152, 475)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 6, 39, Point(148, 275)),
        ExpectedMarkerUpdate(MarkerUpdateType::CHANGE, 9, 357, Point(470, 282)),
        ExpectedMarkerUpdate(MarkerUpdateType::REMOVE, 3, 144, Point(405, 555))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));
}

TEST_F(MarkerTrackerTest, RefreshMatch) {
    auto updates = markerTracker.track(frameOriginal, 0);

    vector<ExpectedMarkerUpdate> expectedUpdates = {
        ExpectedMarkerUpdate(MarkerUpdateType::NEW, 8, 270, Point(282, 342))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates, updates));

    auto updates2 = markerTracker.track(frameUpdatedMatch, 33);

    vector<ExpectedMarkerUpdate> expectedUpdates2 = {
        ExpectedMarkerUpdate(MarkerUpdateType::REMOVE, 8, 270, Point(282, 342)),
        ExpectedMarkerUpdate(MarkerUpdateType::NEW, 6, 90, Point(282, 342))
    };

    ASSERT_TRUE(expectMarkerUpdates(expectedUpdates2, updates2));
}

TEST_F(MarkerTrackerTest, NonConfidentPatternLeakingThrough) {
    auto updates = markerTracker.track(frameBackgroundMatch, 0);

    ASSERT_EQ(5ul, updates.size());

    auto updates2 = markerTracker.track(frameBackgroundMatch, 33);

    ASSERT_EQ(5ul, updates2.size());
}
