#include "markerdetector.hpp"
#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>

namespace mirrors {

    using cv::Size;
    using cv::Vec4i;

    vector<vector<Point>> MarkerDetector::locateMarkers(const Mat& boardImage) const {
        auto thresholdedImage = thresholdGreen(boardImage);
        return findMarkerContours(boardImage, thresholdedImage);
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
            (channels[HSV::H] > 35) & (channels[HSV::H] < 90) &
            // Reasonable saturation of the green color
            (channels[HSV::S] > 40) &
            // Reasonable lightness(not black)
            (channels[HSV::V] > 40);

        // Clean up mask (remove noise, close holes)
        Mat kernel = getStructuringElement(cv::MORPH_RECT, Size(5, 5));
        Mat kernel2 = getStructuringElement(cv::MORPH_RECT, Size(10, 10));
        Mat mask_clean;
        cv::morphologyEx(mask, mask_clean, cv::MORPH_OPEN, kernel);
        cv::morphologyEx(mask_clean, mask, cv::MORPH_CLOSE, kernel);
        cv::morphologyEx(mask, mask_clean, cv::MORPH_OPEN, kernel);

        return mask_clean;
    }

    vector<vector<Point>> MarkerDetector::findMarkerContours(const Mat& original, const Mat& mask) {
        vector<vector<Point>> contours;
        vector<Vec4i> hierarchy;

        // findContours mutates the input, so make a copy
        findContours(Mat(mask), contours, hierarchy, CV_RETR_TREE, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));

        // Find contours that look like markers (first level inner contour)
        vector<vector<Point>> potentialMarkers;

        for (size_t i = 0; i < hierarchy.size(); i++) {
            int parent = hierarchy[i][HierarchyElement::PARENT];

            bool isInnerContour = parent >= 0;
            bool isFirstLevel = isInnerContour && hierarchy[parent][HierarchyElement::PARENT] < 0;

            cv::Rect bb = cv::boundingRect(contours[i]);
            bool largeEnough = bb.width >= 8 && bb.height >= 8;

            if (isInnerContour && isFirstLevel && largeEnough) {
                potentialMarkers.push_back(contours[i]);
            }
        }

        return potentialMarkers;
    }

}
