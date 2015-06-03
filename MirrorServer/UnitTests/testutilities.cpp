#include "testutilities.hpp"

#include <opencv2/highgui/highgui.hpp>
#include "cvutils.hpp"
#include <set>

namespace mirrors {

int countNonZeroMultichannel(Mat input) {
    Mat channels[3];
    split(input, channels);

    return countNonZero(channels[0] & channels[1] & channels[2]);
}

bool imagesApproximatelyEqual(Mat expected, Mat actual, float minimalMatch, float colorDiff) {
    if (expected.rows != actual.rows || expected.cols != actual.cols) {
        return false;
    }

    int absError = countNonZeroMultichannel(cv::abs(expected - actual) < colorDiff);
    float relError = absError / (float) (expected.cols * expected.rows);

    return relError >= minimalMatch;
}

bool expectMarkers(const vector<Point>& expectedPivots, const vector<vector<Point>>& contoursFound, int maxDistance) {
    set<pair<int, int>> foundMarkers;

    for (auto& contour : contoursFound) {
        Point pivot = getPivot(contour);

        for (auto& expect : expectedPivots) {
            if (dist(pivot, expect) <= maxDistance) {
                foundMarkers.insert(make_pair(expect.x, expect.y));
            }
        }
    }

    return foundMarkers.size() == contoursFound.size() && foundMarkers.size() == expectedPivots.size();
}

}
