#ifndef TESTUTILITIES_HPP
#define TESTUTILITIES_HPP

#include <opencv2/highgui/highgui.hpp>

namespace mirrors {

using namespace cv;

int countNonZeroMultichannel(Mat input);

bool imagesApproximatelyEqual(Mat a, Mat b);

}

#endif // TESTUTILITIES_HPP

