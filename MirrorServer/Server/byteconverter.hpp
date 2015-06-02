#ifndef BYTECONVERTER_HPP
#define BYTECONVERTER_HPP

#include <QtEndian>
#include <QByteArray>

/**
 * @file byteconverter.hpp
 * @brief Provides byte conversion functions
 *
 * This file provides functions to convert values to and from
 * raw byte representations. The byte representations are assumed
 * to have big-endian byte order. This makes these functions
 * sufficient for converting number values to send over network
 * connections.
 */

namespace mirrors {
namespace bytes {

/**
 * @brief Converts the value to a byte array.
 *
 * The bytes in the array are converted to big-endian
 * if required.
 *
 * @param value - The value to interpret as a byte array.
 */
template <typename T>
QByteArray toRawBytes(T value) {
    uchar buffer[sizeof(T)];
    qToBigEndian(value, buffer);
    return QByteArray(reinterpret_cast<char*>(buffer), sizeof(T));
}

/**
 * @brief Converts a byte array to a value type.
 *
 * The bytes are first converted to the platform byte order
 * (from big-endian) if required.
 *
 * @param array  - The QByteArray with the bytes.
 * @param offset - The location of the first byte to convert.
 */
template <typename T>
T fromRawBytes(QByteArray array, int offset) {
    Q_ASSERT(array.size() >= offset + sizeof(T));
    const char* data = array.right(array.size() - offset).constData();
    T result = qFromBigEndian(reinterpret_cast<const uchar*>(data));
    return result;
}

}
}

#endif // BYTECONVERTER_HPP

