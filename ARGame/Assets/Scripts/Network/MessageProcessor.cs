//----------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using System.Net;
    using UnityEngine;

    /// <summary>
    /// A class that processes the data sent over the network
    /// to create useful updates.
    /// </summary>
    public static class MessageProcessor
    {
        /// <summary>
        /// Reads a <c>Update</c> type PositionUpdate message.
        /// </summary>
        /// <param name="buffer">The byte array containing data.</param>
        /// <param name="length">The length of the message.</param>
        /// <returns>The PositionUpdate.</returns>
        public static PositionUpdate ReadUpdatePosition(byte[] buffer, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (length < 16)
            {
                return null;
            }

            float x = MessageProcessor.ReadFloat(buffer, 0);
            float y = MessageProcessor.ReadFloat(buffer, 4);
            Vector2 coordinate = new Vector2(x, y);
            float rotation = MessageProcessor.ReadFloat(buffer, 8);
            int id = MessageProcessor.ReadInt(buffer, 12);
            return new PositionUpdate(UpdateType.UpdatePosition, coordinate, rotation, id);
        }

        /// <summary>
        /// Reads a <c>Delete</c> type PositionUpdate message.
        /// </summary>
        /// <param name="buffer">The byte array containing data.</param>
        /// <param name="length">The length of the message.</param>
        /// <returns>The PositionUpdate.</returns>
        public static PositionUpdate ReadDelete(byte[] buffer, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (length < 4)
            {
                return null;
            }

            int id = MessageProcessor.ReadInt(buffer, 0);
            return new PositionUpdate(UpdateType.DeletePosition, new Vector2(0, 0), 0, id);
        }

        /// <summary>
        /// Reads a <c>UpdateRotation</c> type RotationUpdate message.
        /// </summary>
        /// <param name="buffer">The byte array containing data.</param>
        /// <param name="length">The length of the message.</param>
        /// <returns>The RotationUpdate.</returns>
        public static RotationUpdate ReadUpdateRotation(byte[] buffer, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (length < 8)
            {
                return null;
            }

            int id = MessageProcessor.ReadInt(buffer, 0);
            float rotation = MessageProcessor.ReadFloat(buffer, 4);
            return new RotationUpdate(UpdateType.UpdateRotation, rotation, id);
        }

        /// <summary>
        /// Reads and returns a float value from a network byte input, starting
        /// from the given offset.
        /// </summary>
        /// <param name="buffer">The byte array containing data from the network.</param>
        /// <param name="offset">The given offset.</param>
        /// <returns>The float that represents the bytes read.</returns>
        public static float ReadFloat(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 && offset > buffer.Length - 4)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "The offset should be at least 0, and no greater than the size of the buffer minus 4.");
            }

            byte[] bytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset)));
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reads and returns an integer value from a network byte input, starting
        /// from the given offset.
        /// </summary>
        /// <param name="buffer">The byte array containing data from the network.</param>
        /// <param name="offset">The given offset.</param>
        /// <returns>The integer that represents the bytes read.</returns>
        public static int ReadInt(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 && offset > buffer.Length - 4)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "The offset should be at least 0, and no greater than the size of the buffer minus 4.");
            }

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset));
        }
    }
}
