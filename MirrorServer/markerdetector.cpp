#include "markerdetector.hpp"
#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace mirrors {

    using cv::Size;
    using cv::Vec4i;

    vector<vector<Point>> MarkerDetector::locateMarkers(const Mat& boardImage) const {
        auto thresholdedImage = thresholdGreen(boardImage);
        return findMarkerContours(thresholdedImage);
    }

    Mat MarkerDetector::thresholdGreen(const Mat& image) {
        // Convert image to HSV channels
        Mat imageHSV;
        cvtColor(image, imageHSV, CV_BGR2HSV);

        // Split image into H, S, V components
        Mat channels[3];
        split(imageHSV, channels);

        // Threshold on greenish hue, reasonable lightness and at least some saturation
        Mat mask =
            // Threshold on greenish hue
            channels[HSV::H] > 35 & channels[HSV::H] < 90 &
            // Reasonable saturation of the green color
            channels[HSV::S] > 30 &
            // Reasonable lightness(not black and not white)
            channels[HSV::V] > 40 & channels[HSV::V] < 200;

        // Clean up mask (remove noise, close holes)
        Mat kernel = getStructuringElement(cv::MORPH_RECT, Size(5, 5));
        Mat kernel2 = getStructuringElement(cv::MORPH_RECT, Size(10, 10));
        Mat mask_clean;
        cv::morphologyEx(mask, mask_clean, cv::MORPH_OPEN, kernel);
        cv::morphologyEx(mask_clean, mask, cv::MORPH_CLOSE, kernel);

        return mask;
    }

    vector<vector<Point>> MarkerDetector::findMarkerContours(const Mat& mask) {
        vector<vector<Point>> contours;
        vector<Vec4i> hierarchy;

        // findContours mutates the input, so make a copy
        findContours(Mat(mask), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));

        // Find contours that look like markers (outer contour with exactly one inner contour = child)
        vector<vector<Point>> potentialMarkers;

        for (size_t i = 0; i < hierarchy.size(); i++) {
            if (hierarchy[i][HierarchyElement::PARENT] < 0) {
                size_t children = 0;

                for (size_t j = 0; j < hierarchy.size(); j++) {
                    if (hierarchy[j][HierarchyElement::PARENT] == (int) i) {
                        children++;
                    }
                }

                if (children == 1) {
                    potentialMarkers.push_back(contours[i]);
                }
            }
        }

        return potentialMarkers;
    }

}