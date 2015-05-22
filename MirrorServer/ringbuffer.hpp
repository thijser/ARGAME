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

template <typename T>
class ringbuffer {
public:
    ringbuffer(size_t capacity = 10) : capacity(capacity) {}

    void add(const T& t) {
        storage.push_back(t);

        if (storage.size() > capacity) {
            storage.erase(storage.begin());
        }
    }

    size_t size() const {
        return storage.size();
    }

    const std::vector<T> data() const {
        return storage;
    }

private:
    std::vector<T> storage;
    size_t capacity;
};

#endif