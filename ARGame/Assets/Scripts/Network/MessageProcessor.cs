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
    using UnityEngine.Assertions;

    /// <summary>
    /// A class that processes the data sent over the network
    /// to create useful updates.
    /// </summary>
    public static class MessageProcessor
    {
        /// <summary>
        /// Reads a <c>UpdatePosition</c> type PositionUpdate message.
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

            if (buffer.Length < length)
            {
                throw new ArgumentOutOfRangeException("buffer", buffer, "The buffer is not long enough to contain a message of the specified length.");
            }

            if (length < 16)
            {
                return null;
            }

            float x = MessageProcessor.ReadSingle(buffer, 0);
            float y = MessageProcessor.ReadSingle(buffer, 4);
            Vector2 coordinate = new Vector2(x, y);
            float rotation = MessageProcessor.ReadSingle(buffer, 8);
            int id = MessageProcessor.ReadInt32(buffer, 12);

            PositionUpdate update = new PositionUpdate(UpdateType.UpdatePosition, coordinate, rotation, id);
			if(id > 200 || id < 0)
            {
				throw new ArgumentOutOfRangeException("Received message is not sane: " + update);
			}

            return update;
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

            if (buffer.Length < length)
            {
                throw new ArgumentOutOfRangeException("buffer", buffer, "The buffer is not long enough to contain a message of the specified length.");
            }

            if (length < 4)
            {
                return null;
            }

            int id = MessageProcessor.ReadInt32(buffer, 0);
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

            if (buffer.Length < length)
            {
                throw new ArgumentOutOfRangeException("buffer", buffer, "The buffer is not long enough to contain a message of the specified length.");
            }

            if (length < 8)
            {
                return null;
            }

            int id = MessageProcessor.ReadInt32(buffer, 0);
            float rotation = MessageProcessor.ReadSingle(buffer, 4);
            return new RotationUpdate(UpdateType.UpdateRotation, rotation, id);
        }

        /// <summary>
        /// Reads a <c>UpdateRotation</c> type RotationUpdate message.
        /// </summary>
        /// <param name="buffer">The byte array containing data.</param>
        /// <param name="length">The length of the message.</param>
        /// <returns>The RotationUpdate.</returns>
        public static LevelUpdate ReadUpdateLevel(byte[] buffer, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length < length)
            {
                throw new ArgumentOutOfRangeException("buffer", buffer, "The buffer is not long enough to contain a message of the specified length.");
            }

            if (length < 12)
            {
                return null;
            }

            int index = MessageProcessor.ReadInt32(buffer, 0);
            float width = MessageProcessor.ReadSingle(buffer, 4);
            float height = MessageProcessor.ReadSingle(buffer, 8);

            return new LevelUpdate(index, new Vector2(width, height));
        }

        /// <summary>
        /// Reads an <see cref="ARViewUpdate"/> message from the given byte array.
        /// </summary>
        /// <param name="bytes">The byte array to read from.</param>
        /// <param name="length">The maximum amount of bytes to read.</param>
        /// <returns></returns>
        public static ARViewUpdate ReadARViewUpdate(byte[] bytes, int length)
        {
            Assert.IsNotNull(bytes);
            Assert.IsFalse(length < 28, "ARViewUpdate length needs to be at least 28");
            Assert.IsFalse(bytes.Length < length, "byte array length is insufficient");

            int id = MessageProcessor.ReadInt32(bytes, 0);
            Vector3 position = new Vector3(
                MessageProcessor.ReadSingle(bytes, 4),
                MessageProcessor.ReadSingle(bytes, 8),
                MessageProcessor.ReadSingle(bytes, 12));
            Vector3 rotation = new Vector3(
                MessageProcessor.ReadSingle(bytes, 16),
                MessageProcessor.ReadSingle(bytes, 20),
                MessageProcessor.ReadSingle(bytes, 24));

            return new ARViewUpdate(id, position, rotation);
        }

        /// <summary>
        /// Writes the given <see cref="RotationUpdate"/> to a byte array.
        /// </summary>
        /// <param name="update">The <see cref="RotationUpdate"/>, not null.</param>
        /// <returns>A byte array containing the data from the <see cref="RotationUpdate"/>.</returns>
        public static byte[] WriteRotationUpdate(RotationUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            byte[] message = new byte[9];
            message[0] = (byte)UpdateType.UpdateRotation;
            WriteInt32(update.Id, message, 1);
            WriteSingle(update.Rotation, message, 5);
            return message;
        }

        /// <summary>
        /// Writes the given <see cref="LevelUpdate"/> to a byte array.
        /// </summary>
        /// <param name="update">The <see cref="LevelUpdate"/>, not null.</param>
        /// <returns>A byte array containing the data from the <see cref="LevelUpdate"/>.</returns>
        public static byte[] WriteLevelUpdate(LevelUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            byte[] message = new byte[13];

            message[0] = (byte)UpdateType.UpdateLevel;
            WriteInt32(update.NextLevelIndex, message, 1);
            WriteSingle(update.Size.x, message, 5);
            WriteSingle(update.Size.y, message, 9);
            return message;
        }

        /// <summary>
        /// Writes the given <see cref="ARViewUpdate"/> to a byte array.
        /// </summary>
        /// <param name="update">The <see cref="ARViewUpdate"/>, not null.</param>
        /// <returns>A byte array containing the data from the <see cref="ARViewUpdate"/>.</returns>
        public static byte[] WriteARViewUpdate(ARViewUpdate update)
        {
            Assert.IsNotNull(update);

            byte[] message = new byte[29];
            message[0] = (byte)UpdateType.UpdateARView;
            WriteInt32(update.Id, message, 1);

            WriteSingle(update.Position.x, message, 5);
            WriteSingle(update.Position.y, message, 9);
            WriteSingle(update.Position.z, message, 13);

            WriteSingle(update.Rotation.x, message, 17);
            WriteSingle(update.Rotation.y, message, 21);
            WriteSingle(update.Rotation.z, message, 25);

            return message;
        }

        /// <summary>
        /// Reads and returns a float value from a network byte input, starting
        /// from the given offset.
        /// </summary>
        /// <param name="buffer">The byte array containing data from the network.</param>
        /// <param name="offset">The given offset.</param>
        /// <returns>The float that represents the bytes read.</returns>
        public static float ReadSingle(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length - 4)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "The offset should be at least 0, and no greater than the size of the buffer minus 4.");
            }

            byte[] bytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset)));
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Writes a float to a byte array.
        /// <para>
        /// The value is stored in the buffer in network byte order.
        /// </para>
        /// </summary>
        /// <param name="value">The float value to write.</param>
        /// <param name="buffer">The buffer to store the value in, not null.</param>
        /// <param name="offset">The starting position where to store the value.</param>
        /// <exception cref="ArgumentNullException">If the buffer is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the offset is out of range.</exception>
        public static void WriteSingle(float value, byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length - 4)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "The offset should be at least 0, and no greater than the size of the buffer minus 4.");
            }

            int networkValue = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
            byte[] bytes = BitConverter.GetBytes(networkValue);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        /// <summary>
        /// Writes an integer to a byte array.
        /// <para>
        /// The value is stored in the buffer in network byte order.
        /// </para>
        /// </summary>
        /// <param name="value">The integer value to write.</param>
        /// <param name="buffer">The buffer to store the value in, not null.</param>
        /// <param name="offset">The starting position where to store the value.</param>
        /// <exception cref="ArgumentNullException">If the buffer is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the offset is out of range.</exception>
        public static void WriteInt32(int value, byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length - 4)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "The offset should be at least 0, and no greater than the size of the buffer minus 4.");
            }

            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        /// <summary>
        /// Reads and returns an integer value from a network byte input, starting
        /// from the given offset.
        /// </summary>
        /// <param name="buffer">The byte array containing data from the network.</param>
        /// <param name="offset">The given offset.</param>
        /// <returns>The integer that represents the bytes read.</returns>
        public static int ReadInt32(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length - 4)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "The offset should be at least 0, and no greater than the size of the buffer minus 4.");
            }

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset));
        }
    }
}
