#include "testutilities.hpp"

namespace mirrors {

int countNonZeroMultichannel(Mat input) {
    Mat channels[3];
    split(input, channels);

    return countNonZero(channels[0] & channels[1] & channels[2]);
}

bool imagesApproximatelyEqual(Mat a, Mat b) {
    if (a.rows != b.rows || a.cols != b.cols) {
        return false;
    }

    int absError = countNonZeroMultichannel(cv::abs(a - b) < 10);
    float relError = absError / (float) (a.cols * a.rows);

    return relError >= 0.95f;
}

}
