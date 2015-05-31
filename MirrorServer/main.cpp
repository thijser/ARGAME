/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

#include <iostream>
#include <atomic>
#include <opencv2/highgui/highgui.hpp>
#include "markertracker.hpp"
#include "serversocket.hpp"

using namespace mirrors;

/// Port used for the server socket.
const uint16_t SERVER_PORT = 23369;

/// Horizontal capture resolution for camera.
const int CAMERA_WIDTH = 1600;

/// Vertical capture resolution for camera.
const int CAMERA_HEIGHT = 896;

/// Directory that contains marker images.
const std::string MARKER_DIRECTORY = "markers/";

/// Amount of markers.
const int MARKER_COUNT = 12;

/// Informational title of window for locating board.
const std::string BOARD_FINDING_WINDOW_TITLE = "Position the camera to capture the full board";

/// Code for escape returned by cv::waitKey.
const int KEY_ESCAPE = 27;

/**
* @brief Dummy function for handling SIGPIPE, required
* to stop server crashes on a broken pipe (non-Windows).
*/
void handleSigpipe(int) {
#ifdef MIRRORS_DEBUG
    std::clog << "DEBUG: Received SIGPIPE: " << std::endl;
#endif
}

/**
* @brief Entry point of this application.
*
* The first argument to the application indicates the
* camera device ID. If omitted, 0 is used as device ID.
*
* The second argument indicates if frames are displayed
* or not. A non-zero value indicates frames should be
* displayed, while a value of 0 skips showing the frames.
* The default value is to show the frames.
*
* @param argc - The amount of command-line arguments.
* @param argv - The command-line argument values.
* @return The exit code of this application.
*/
int main(int argc, char** argv) {
#ifndef WIN32
    signal(SIGPIPE, handleSigpipe);
#endif

    // Parse command line arguments
    int deviceId = 0;

    if (argc > 1) deviceId = atoi(argv[1]);

#ifdef MIRRORS_DEBUG
    std::clog << "Using camera device #" << deviceId << std::endl;
#endif

    // Initialize camera
    cv::VideoCapture captureDevice(deviceId);

    if (!captureDevice.isOpened()) {
        std::cerr << "Couldn't open specified capture device!" << std::endl;
        return EXIT_FAILURE;
    }

    captureDevice.set(CV_CAP_PROP_FRAME_WIDTH, CAMERA_WIDTH);
    captureDevice.set(CV_CAP_PROP_FRAME_HEIGHT, CAMERA_HEIGHT);

#ifdef MIRRORS_DEBUG
    // Check if requested resolution is available
    int actualWidth = (int) captureDevice.get(CV_CAP_PROP_FRAME_WIDTH);
    int actualHeight = (int) captureDevice.get(CV_CAP_PROP_FRAME_HEIGHT);

    if (actualWidth != CAMERA_WIDTH || actualHeight != CAMERA_HEIGHT) {
        std::clog << "Camera resolution " << CAMERA_WIDTH << " x " << CAMERA_HEIGHT << " not supported!" << std::endl;
        std::clog << "Falling back to " << actualWidth << " x " << actualHeight << "." << std::endl;
    }
#endif

    // Set up detectors and recognizers using default techniques
    BoardDetector boardDetector;
    MarkerDetector markerDetector;
    MarkerRecognizer markerRecognizer;

    for (int i = 0; i < MARKER_COUNT; i++) {
        auto pattern = cv::imread("markers/" + std::to_string(i) + ".png", CV_LOAD_IMAGE_GRAYSCALE);
        
        if (!markerRecognizer.registerPattern(i, pattern)) {
            std::cerr << "Failed to load marker \"markers/" << i << ".png\"!" << std::endl;
            return EXIT_FAILURE;
        }
    }

    // Locate the board
    Mat frame;

    cv::startWindowThread();
    cv::namedWindow(BOARD_FINDING_WINDOW_TITLE);

    do {
        captureDevice.read(frame);
        cv::imshow(BOARD_FINDING_WINDOW_TITLE, frame);
        
        if (cv::waitKey(1) == KEY_ESCAPE) {
            return EXIT_FAILURE;
        }
    } while (!boardDetector.locateBoard(frame));

    cv::destroyWindow(BOARD_FINDING_WINDOW_TITLE);

    // Set up marker tracker
    MarkerTracker markerTracker(boardDetector, markerDetector, markerRecognizer);

    // Initialize server
    std::atomic<bool> running = true;

    ServerSocket::initialize();
    ServerSocket server(SERVER_PORT);
    std::thread serverThread([&]() {
        try {
            server.run();
        } catch (const NL::Exception& ex) {
            std::cerr << "Uncaught exception in server: " << ex.what() << std::endl;
            running = false;
        }
    });

    // Start capture & detect loop
    cv::namedWindow("Camera", CV_WINDOW_AUTOSIZE);

    while (running) {
        // Grab next frame from camera
        Mat frame;
        captureDevice.read(frame);

        // Process marker changes and propagate them to the clients
        auto updates = markerTracker.track(frame);

        for (auto& update : updates) {
            if (update.type == MarkerUpdateType::NEW || update.type == MarkerUpdateType::CHANGE) {
                // Scale marker positions based on their size for Meta 1 tracking
                cv::Point2f scaledCoords(update.position.x / update.scale, update.position.y / update.scale);

                server.broadcastPositionUpdate(update.id, scaledCoords.x, scaledCoords.y, update.rotation);
            } else if (update.type == MarkerUpdateType::REMOVE) {
                server.broadcastDelete(update.id);
            }
        }

        // Show latest frame to user
        cv::imshow("Camera", boardDetector.extractBoard(frame));
        
        if (cv::waitKey(1) == KEY_ESCAPE) {
            break;
        }
    }

    // Disconnect the socket and wait for the server and detector to stop
    server.disconnect();
    serverThread.join();

    return EXIT_SUCCESS;
}
