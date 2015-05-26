/*
* Copyright 2015, Delft University of Technology
*
* This software is licensed under the terms of the MIT license.
* See http://opensource.org/licenses/MIT for the full license.
*
*
*/

#ifndef RINGBUFFER_HPP
#define RINGBUFFER_HPP

#include <vector>

/**
 * A circular buffer.
 *
 * Collection with a maximum capacity where the first inserted item is
 * removed for every newly inserted item once the collection is full.
 */
template <typename T>
class ringbuffer {
public:
    /**
     * @brief Creates a circular buffer with the specified capacity.
     * @param capacity - Maximum amount of items the buffer will hold.
     */
    ringbuffer(size_t capacity = 10) : capacity(capacity) {}

    /**
     * @brief Add an item to the buffer and remove out the oldest item
     * if the buffer already contains *capacity* items.
     * @param t - Item to add.
     */
    void add(const T& t) {
        storage.push_back(t);

        if (storage.size() > capacity) {
            storage.erase(storage.begin());
        }
    }

    /**
     * @brief Returns the amount of items contained within the buffer.
     * @return The amount of items contained in the buffer (<= capacity).
     */
    size_t size() const {
        return storage.size();
    }

    /**
     * @brief Returns a collection of the items in the buffer.
     * @return Vector representing the items in the buffer.
     */
    const std::vector<T> data() const {
        return storage;
    }

private:
    /// Underlying container of buffer.
    std::vector<T> storage;

    /// Maximum capacity of buffer.
    size_t capacity;
};

#endif