/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

/**
* @file averager.hpp
* @brief Contains implementation of mirrors::Averager;
*/

#ifndef AVERAGER_HPP
#define AVERAGER_HPP

#include <deque>

namespace mirrors {

    using std::deque;

    /**
     * @brief Calculates a moving average with fast response for quickly changing values.
     * The functionality of this class allows for smoothing measurements while still
     * immediately responding to sudden changes.
     */
    template <typename T>
    class Averager {
    public:
        /**
         * @brief Create moving average calculator with specified properties.
         * @param history - Amount of values to average.
         * @param flushDistance - Minimum distance between average and new value
         * for triggering fast response where history is cleared and the new
         * value is used immediately.
         */
        Averager(size_t history, T flushDistance = std::numeric_limits<T>::max())
            : history(history), flushDistance(flushDistance) {}

        T update(T newValue) {
            // Add new value to history and pop oldest item if history is full
            storage.push_back(newValue);
            if (storage.size() > history) storage.pop_front();

            // Calculate new moving average
            T average = 0;

            for (T val : storage) {
                average += val;
            }

            average /= storage.size();

            // If average deviates too much from new value, then flush history
            if (std::abs(average - newValue) > flushDistance) {
                average = newValue;

                storage.clear();
                storage.push_back(average);
            }

            return average;
        }

    private:
        deque<T> storage;
        size_t history;
        T flushDistance;
    };

}

#endif