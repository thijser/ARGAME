#include "testutilities.hpp"

#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include "cvutils.hpp"
#include <set>

namespace mirrors {

int countNonZeroMultichannel(Mat input) {
    Mat channels[3];
    split(input, channels);

    return countNonZero(channels[0] & channels[1] & channels[2]);
}

Mat normalizeBoard(const Mat& raw) {
    Mat result;
    cv::resize(raw, result, Size(570, 720));
    return result;
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

bool expectMarkerUpdates(const vector<ExpectedMarkerUpdate>& expectedUpdates, const vector<MarkerUpdate>& updates) {
    set<size_t> matchedUpdates;

    for (auto& update : updates) {
        for (size_t i = 0; i < expectedUpdates.size(); i++) {
            if (expectedUpdates[i].matches(update)) {
                matchedUpdates.insert(i);
            }
        }
    }

    return matchedUpdates.size() == updates.size() && matchedUpdates.size() == expectedUpdates.size();
}

int loadPatterns(MarkerRecognizer& recognizer, const std::string& path) {
    int i = 0;
    bool loaded;

    do {
        vector<char> buf;
        buf.resize(path.size() + 20);
        sprintf_s(&buf[0], buf.size(), path.c_str(), i);
        loaded = recognizer.registerPattern(i, imread(buf.data(), IMREAD_GRAYSCALE));
        i++;
    } while (loaded);

    return i - 1;
}

}
