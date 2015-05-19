#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <iostream>

using namespace cv;
using namespace std;

int main(int, char**) {
    VideoCapture cap(0);

    namedWindow("MyVideo", CV_WINDOW_AUTOSIZE);

    int w = cap.get(CV_CAP_PROP_FRAME_WIDTH);
    int h = cap.get(CV_CAP_PROP_FRAME_HEIGHT);

    while (true) {
        Mat frame, frame2;
        cap.read(frame);

        // Correct perspective of surface
        Point2f src[] = {Point2f(140, 0), Point2f(520, 0), Point2f(73, 479), Point2f(586, 479)};
        Point2f dst[] = {Point2f(0, 0), Point2f(640, 0), Point2f(0, 480), Point2f(640, 480)};
        Mat m = getPerspectiveTransform(src, dst);

        warpPerspective(frame, frame2, m, cv::Size(w, h));
        resize(frame2, frame, cv::Size(21 * 30, 24 * 30));

        // Threshold green markers
        Mat frameParts[3];
        split(frame, frameParts);

        inRange(frameParts[1], cv::Scalar(60), cv::Scalar(255), frame);
        inRange(frameParts[0], cv::Scalar(60), cv::Scalar(255), frame2);

        Mat result = frame & ~frame2;
        Mat result2;

        auto kernel = getStructuringElement(MORPH_RECT, cv::Size(3, 3));
        erode(result, result2, kernel);

        auto kernel2 = getStructuringElement(MORPH_RECT, cv::Size(9, 9));
        dilate(result2, result, kernel2);

        // Find edges
        Canny(result, result2, 4, 8, 3);

        // Find markers
        vector<vector<Point>> contours;

        findContours(result, contours, CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));

        std::cout << "found " << contours.size() << " markers" << std::endl;

        // Show result
        imshow("MyVideo", result2);

        if (waitKey(10) == 27) {
            break;
        }
    }

    return 0;
}
