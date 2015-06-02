#include "markerrecognizer.hpp"

#include <opencv2/imgproc/imgproc.hpp>
#include <cstdint>

namespace mirrors {

    bool MarkerRecognizer::registerPattern(int id, const Mat& pattern) {
        // Check if marker is 6 x 6 and a grayscale image
        if (id >= 0 && pattern.rows == 6 && pattern.cols == 6 && pattern.channels() == 1) {
            patterns[id] = pattern;
            return true;
        } else {
            return false;
        }
    }

    PatternMatch MarkerRecognizer::recognizeMarker(const Mat& boardImage, const vector<Point>& contour) const {
        // Straighten marker
        auto straightenedMarker = straightenMarker(boardImage, contour);

        // Isolate the 6 x 6 pattern in the marker
        auto pattern = extractPattern(straightenedMarker.first);

        // Find the closest matching known pattern and its n * 90 degree rotation
        auto patternMatch = findMatchingPattern(pattern);

        // Add the rotation needed for straightening to the match and normalize it
        patternMatch.rotation = straightenedMarker.second - patternMatch.rotation;

        if (patternMatch.rotation < 0) {
            patternMatch.rotation += 360;
        }

        // Determine scale by averaging width/height (marker is square)
        int w = straightenedMarker.first.cols;
        int h = straightenedMarker.first.rows;

        patternMatch.scale = ((w + h) / 2) / 8.0f;

        return patternMatch;
    }

    PatternMatch MarkerRecognizer::findMatchingPattern(const Mat& input) const {
        // Calculate all permutations of input pattern
        vector<std::pair<ExactAngle, Mat>> inputPermutations = {
            std::make_pair(CLOCKWISE_0, input),
            std::make_pair(CLOCKWISE_90, rotateExactly(input, CLOCKWISE_90)),
            std::make_pair(CLOCKWISE_180, rotateExactly(input, CLOCKWISE_180)),
            std::make_pair(CLOCKWISE_270, rotateExactly(input, CLOCKWISE_270))
        };

        PatternMatch bestMatch;

        for (auto& knownPattern : patterns) {
            for (auto& permutation : inputPermutations) {
                float confidence = (float) cv::countNonZero(permutation.second == knownPattern.second);
                confidence /= input.rows * input.cols;

                if (confidence > bestMatch.confidence) {
                    bestMatch = PatternMatch(knownPattern.first, (float) permutation.first, confidence);
                }
            }
        }

        return bestMatch;
    }

    pair<Mat, float> MarkerRecognizer::straightenMarker(const Mat& boardImage, const vector<Point>& contour) {
        cv::RotatedRect rotatedRect = cv::minAreaRect(contour);

        // Take part of the image containing the marker pattern and straighten it
        cv::Rect bb = rotatedRect.boundingRect();

        // Clamp bounding rect to board borders
        if (bb.x < 0) bb.x = 0;
        if (bb.y < 0) bb.y = 0;
        if (bb.x + bb.width > boardImage.cols) bb.width = boardImage.cols - bb.x;
        if (bb.y + bb.height > boardImage.rows) bb.height = boardImage.rows - bb.y;

        Mat marker = boardImage(bb);
        marker = rotateImage(marker, -rotatedRect.angle);

        // Crop image to remove the padding caused by the former rotation
        // ____________         ____________
        // |  /      /|      -> | |      | | <-
        // | /      / | -->  -> | |      | | <-
        // |/______/__|      -> |_|______|_| <-
        //
        cv::Rect roi(
            marker.cols / 2 - (int) rotatedRect.size.width / 2,
            marker.rows / 2 - (int) rotatedRect.size.height / 2,
            (int) rotatedRect.size.width,
            (int) rotatedRect.size.height
        );

        marker = marker(roi);

        return std::make_pair(marker, rotatedRect.angle);
    }

    Mat MarkerRecognizer::extractPattern(const Mat& straightenedMarker) {
        // Convert marker image to grayscale
        Mat straightenedMarkerGray;
        cv::cvtColor(straightenedMarker, straightenedMarkerGray, CV_BGR2GRAY);

        // Downsize to 8 x 8
        Mat downsizedMarker;
        cv::resize(straightenedMarkerGray, downsizedMarker, cv::Size(8, 8));

        // Determine average brightness for separating black/white
        int avgBrightness = 0;

        for (int i = 0; i < downsizedMarker.cols; i++) {
            for (int j = 0; j < downsizedMarker.rows; j++) {
                avgBrightness += downsizedMarker.at<uint8_t>(i, j);
            }
        }

        avgBrightness /= downsizedMarker.cols * downsizedMarker.rows;

        // Threshold marker
        Mat thresholdedMarker;
        cv::threshold(downsizedMarker, thresholdedMarker, avgBrightness, 255, 0);

        cv::Rect patternRegion(1, 1, 6, 6);
        return thresholdedMarker(patternRegion);
    }

}
