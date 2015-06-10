#include "markerdetector.hpp"
#include "cvutils.hpp"

#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>

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
            (channels[HSV::H] > 35) & (channels[HSV::H] < 90) &
            // Reasonable saturation of the green color
            (channels[HSV::S] > 40) &
            // Reasonable lightness(not black)
            (channels[HSV::V] > 40);

        // Clean up mask (remove noise, close holes)
        Mat kernel = getStructuringElement(cv::MORPH_RECT, Size(3, 3));
        Mat maskClean;
        cv::morphologyEx(mask, maskClean, cv::MORPH_OPEN, kernel);
        cv::morphologyEx(maskClean, mask, cv::MORPH_CLOSE, kernel);
        cv::morphologyEx(mask, maskClean, cv::MORPH_OPEN, kernel);

        return maskClean;
    }

    vector<vector<Point>> MarkerDetector::findMarkerContours(const Mat& mask) {
        vector<vector<Point>> contours;
        vector<Vec4i> hierarchy;

        // findContours mutates the input, so make a copy
        findContours(Mat(mask), contours, hierarchy, CV_RETR_TREE, CV_CHAIN_APPROX_NONE, Point(0, 0));

        // Find contours that look like markers (first level inner contour)
        vector<vector<Point>> potentialMarkers;
        vector<std::pair<size_t, size_t>> circumferences;

        for (size_t i = 0; i < hierarchy.size(); i++) {
            int parent = hierarchy[i][HierarchyElement::PARENT];

            bool isInnerContour = parent >= 0;
            bool isFirstLevel = isInnerContour && hierarchy[parent][HierarchyElement::PARENT] < 0;

            cv::RotatedRect bb = cv::minAreaRect(contours[i]);
            bool largeEnough = bb.size.width >= 8 && bb.size.height >= 8;
            bool isSquare = std::abs(bb.size.width - bb.size.height) / (float) bb.size.width < 0.15f;

            if (isInnerContour && isFirstLevel && largeEnough && isSquare) {
                potentialMarkers.push_back(contours[i]);
                circumferences.push_back(std::make_pair(contours[i].size(), potentialMarkers.size() - 1));
            }
        }

        if (potentialMarkers.size() > 0) {
            // Calculate median circumference
            std::sort(circumferences.begin(), circumferences.end());
            size_t medianCircumference = circumferences[circumferences.size() / 2].first;

            // Keep just the contours that are close to this median
            // This removes any remaining noise identified as marker.
            vector<vector<Point>> finalPotentialMarkers;

            for (auto& pair : circumferences) {
                if (std::abs((int) medianCircumference - (int) pair.first) / (float) medianCircumference < 0.15f) {
                    finalPotentialMarkers.push_back(potentialMarkers[pair.second]);
                }
            }

            potentialMarkers = finalPotentialMarkers;
        }

        return potentialMarkers;
    }

}
