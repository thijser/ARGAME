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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    /// <summary>
    /// A class that processes the data sent over the network
    /// to create useful updates.
    /// </summary>
    public class MessageProcessor
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="MessageProcessor"/> 
        /// class from being created.
        /// </summary>
        private MessageProcessor()
        { 
        }

        /// <summary>
        /// Reads a <c>Update</c> type PositionUpdate message.
        /// </summary>
        /// <returns>The PositionUpdate.</returns>
        public static PositionUpdate ReadUpdatePosition(byte[] buffer)
        {
            if(buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if(buffer.Length < 16)
            {
                throw new ArgumentException("The supplied byte array is not long enough to contain all the required data.");
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
        /// <returns>The PositionUpdate.</returns>
        public static PositionUpdate ReadDelete(byte[] buffer)
        {
            int id = MessageProcessor.ReadInt(buffer, 0);
            return new PositionUpdate(UpdateType.DeletePosition, new Vector2(0, 0), 0, id);
        }

        /// <summary>
        /// Reads and returns a float value from a network byte input, starting
        /// from the given offset.
        /// </summary>
        /// <param name="buffer">The byte array containing data from the network.</param>
        /// <param name="offset">The given offset.</param>
        /// <returns>The float that represents the bytes read.</returns>
        private static float ReadFloat(byte[] buffer, int offset)
        {
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
        private static int ReadInt(byte[] buffer, int offset)
        {
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset));
        }
    }
}
