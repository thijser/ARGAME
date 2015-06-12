#ifndef TESTUTILITIES_HPP
#define TESTUTILITIES_HPP

#include <opencv2/core/core.hpp>
#include "markertracker.hpp"
#include <vector>

namespace mirrors {
using namespace cv;
using namespace std;

/**
 * @brief Counts the amount of non-zero values in the matrix.
 *
 * This function behaves the same as cv::countNonZero, except that
 * this method properly handles multi-channel images.
 * @param input - The input matrix.
 * @return The amount of non-zero values in the input.
 */
int countNonZeroMultichannel(Mat input);

/**
 * @brief Normalize board image to original 570 x 720 size.
 * @param raw - Raw board image.
 * @return Board image resized to 570 x 720.
 */
Mat normalizeBoard(const Mat& raw);

/**
 * @brief Tests if the two given images are approximately equal.
 * @param expected     - The expected image
 * @param actual       - The actual image
 * @param minimalMatch - The factor of the expected image that should match.
 * @param colorDiff    - The maximum amount that two values may differ to be considered the same.
 * @return True if the images are approximately equal, false otherwise.
 */
bool imagesApproximatelyEqual(Mat expected, Mat actual, float minimalMatch = 0.95f, float colorDiff = 10);

/**
 * @brief Tests if the expected marker centers have been found.
 * @param expectedPivots - Expected pivot points.
 * @param contoursFound - Marker contours that should have matching pivot points.
 * @param maxDistance - Maximum distance between marker contour and closest expected pivot.
 * @return True if all of the expected pivots have been found exactly once.
 */
bool expectMarkers(const vector<Point>& expectedPivots, const vector<vector<Point>>& contoursFound, int maxDistance = 5);

/**
 * @brief Loads patterns into a recognizer based on a path like "markers/%d.png".
 * @param recognizer - Recognizer to load patterns into.
 * @param path - Path with %d that will be replaced with sequence of numbers.
 * @return Amount of patterns loaded.
 */
int loadPatterns(MarkerRecognizer& recognizer, const std::string& path);

/**
 * @brief Describes an expected marker tracking update.
 */
class ExpectedMarkerUpdate {
    MarkerUpdateType::MarkerUpdateType type;
    int id;
    float rotation;
    Point position;

public:
    ExpectedMarkerUpdate(MarkerUpdateType::MarkerUpdateType type, int id, float rotation, Point position)
        : type(type), id(id), rotation(rotation), position(position) {}

    bool matches(const MarkerUpdate& update) const {
        bool m = true;

        m &= update.type == type;
        m &= update.id == id;
        m &= std::abs(update.rotation - rotation) <= 5;
        m &= dist(update.position, position) <= 5;

        return m;
    }
};

/**
 * @brief Checks if the updates match the expected updates (in any order).
 * @param expectedUpdates - Expected updates.
 * @param updates - Actual updates.
 * @return True if updates match expectedUpdates without leftovers on either side.
 */
bool expectMarkerUpdates(const vector<ExpectedMarkerUpdate>& expectedUpdates, const vector<MarkerUpdate>& updates);

}

#endif // TESTUTILITIES_HPP

