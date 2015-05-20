/*
 * Copyright 2015, Delft University of Technology
 *
 * This software is licensed under the terms of the MIT license.
 * See http://opensource.org/licenses/MIT for the full license.
 *
 *
 */

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <iostream>

using namespace cv;
using namespace std;

int main(int, char**) {
    VideoCapture cap(0);

    namedWindow("Master Camera", CV_WINDOW_AUTOSIZE);

    int w = cap.get(CV_CAP_PROP_FRAME_WIDTH);
    int h = cap.get(CV_CAP_PROP_FRAME_HEIGHT);

    while (true) {
        Mat frame, frame2;
        cap.read(frame);

        // Correct perspective of surface
        Point2f src[] = {Point2f(171, 2), Point2f(518, 2), Point2f(121, 477), Point2f(586, 475)};
        Point2f dst[] = {Point2f(0, 0), Point2f(640, 0), Point2f(0, 480), Point2f(640, 480)};
        Mat m = getPerspectiveTransform(src, dst);

        warpPerspective(frame, frame2, m, cv::Size(w, h));
        resize(frame2, frame, cv::Size(19 * 30, 24 * 30));

        Mat original = frame;

        // Threshold green markers
        Mat frameParts[3];
        split(frame, frameParts);

        inRange(frameParts[1], cv::Scalar(50), cv::Scalar(255), frame);
        inRange(frameParts[0], cv::Scalar(60), cv::Scalar(255), frame2);

        Mat result = frame & ~frame2 & (frameParts[1] > frameParts[2]);
        Mat result2;

        auto kernel = getStructuringElement(MORPH_RECT, cv::Size(3, 3));
        erode(result, result2, kernel);

        auto kernel2 = getStructuringElement(MORPH_RECT, cv::Size(10, 10));
        dilate(result2, result, kernel2);

        // Find edges
        Canny(result, result2, 4, 8, 3);

        // Find markers
        vector<vector<Point>> contours;
        vector<Vec4i> hierarchy;

        findContours(result, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));

        // Keep the markers that have exactly one hole in them
        vector<int> potentialMarkers;

        for (int i = 0; i < hierarchy.size(); i++) {
            // If contour has no parent (outer rectangle), find if it has exactly one child
            if (hierarchy[i][3] == -1) {
                int children = 0;

                for (int j = 0; j < hierarchy.size(); j++) {
                    if (hierarchy[j][3] == i) {
                        children++;
                    }
                }

                if (children == 1) {
                    potentialMarkers.push_back(i);
                }
            }
        }

        std::cout << "found " << potentialMarkers.size() << " potential markers" << std::endl;

        /*for (auto& contour : contours) {
            if (contour.size() >= 5) {
                cv::RotatedRect rect = fitEllipse(contour);
                std::cout << (int) rect.angle << std::endl;
                break;
            }
        }*/

        // Show result
        imshow("Master Camera", result);

        if (waitKey(10) == 27) {
            break;
        }

        //break;
    }

    return 0;
}
