#ifndef TESTUTILITIES_HPP
#define TESTUTILITIES_HPP

#include <opencv2/core/core.hpp>

namespace mirrors {
using namespace cv;

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
 * @brief Tests whether the two given images are approximately equal.
 * @param expected     - The expected image
 * @param actual       - The actual image
 * @param minimalMatch - The factor of the expected image that should match.
 * @param colorDiff    - The maximum amount that two values may differ to be considered the same.
 * @return True if the images are approximately equal, false otherwise.
 */
bool imagesApproximatelyEqual(Mat expected, Mat actual, float minimalMatch = 0.95f, float colorDiff = 10);

}

#endif // TESTUTILITIES_HPP

