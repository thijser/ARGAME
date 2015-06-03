#ifndef TESTUTILITIES_HPP
#define TESTUTILITIES_HPP

#include <opencv2/core/core.hpp>
#include "markerrecognizer.hpp"
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

}

#endif // TESTUTILITIES_HPP

