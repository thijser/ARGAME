//----------------------------------------------------------------------------
// <copyright file="MessageProcessorTest.cs" company="Delft University of Technology">
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
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Test class for MarkerState.
    /// </summary>
    [TestFixture]
    public class MessageProcessorTest : MirrorsUnitTest
    {
        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadUpdatePositionNull()
        {
            MessageProcessor.ReadUpdatePosition(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadUpdateRotationNull()
        {
            MessageProcessor.ReadUpdateRotation(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadDeleteNull()
        {
            MessageProcessor.ReadDelete(null, 0);
        }

        /// <summary>
        /// Tests if the correct exception if thrown when null
        /// is passed as the buffer argument.
        /// </summary>
        /// <exception cref="ArgumentNullException">Expected Exception.</exception>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadLevelUpdateNullBuffer()
        {
            MessageProcessor.ReadUpdateLevel(null, 12);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadIntNull()
        {
            MessageProcessor.ReadInt(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadFloatNull()
        {
            MessageProcessor.ReadFloat(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when length
        /// is insufficient.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadUpdatePositionInvalidLength()
        {
            MessageProcessor.ReadUpdatePosition(new byte[16], 1000);
        }

        /// <summary>
        /// Test if correct exception is thrown when length
        /// is insufficient.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadUpdateRotationInvalidLength()
        {
            MessageProcessor.ReadUpdateRotation(new byte[16], 1000);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the length
        /// is larger than the buffer size.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected Exception.</exception>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadLevelUpdateInvalidLength()
        {
            MessageProcessor.ReadUpdateLevel(new byte[12], 13);
        }

        /// <summary>
        /// Test if correct exception is thrown when length
        /// is insufficient.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadDeleteInvalidLength()
        {
            MessageProcessor.ReadDelete(new byte[16], 1000);
        }

        /// <summary>
        /// Test if correct exception is thrown when offset is illegal.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadIntNegative()
        {
            MessageProcessor.ReadInt(new byte[16], -1);
        }

        /// <summary>
        /// Test if correct exception is thrown when offset is illegal.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadIntBufferTooSmall()
        {
            MessageProcessor.ReadInt(new byte[16], 13);
        }

        /// <summary>
        /// Test if correct exception is thrown when offset is illegal.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadFloatNegative()
        {
            MessageProcessor.ReadFloat(new byte[16], -1);
        }

        /// <summary>
        /// Test if correct exception is thrown when offset is illegal.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadFloatBufferTooSmall()
        {
            MessageProcessor.ReadFloat(new byte[16], 13);
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is insufficient.
        /// </summary>
        [Test]
        public void TestReadUpdatePositionBufferTooSmall()
        {
            Assert.Null(MessageProcessor.ReadUpdatePosition(new byte[16], 15));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is insufficient.
        /// </summary>
        [Test]
        public void TestReadUpdateRotationBufferTooSmall()
        {
            Assert.Null(MessageProcessor.ReadUpdateRotation(new byte[16], 7));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is insufficient.
        /// </summary>
        [Test]
        public void TestReadDeleteBufferTooSmall()
        {
            Assert.Null(MessageProcessor.ReadDelete(new byte[16], 3));
        }

        /// <summary>
        /// Tests if null is returned if the length of the message is insufficient.
        /// </summary>
        [Test]
        public void TestReadLevelUpdateBufferTooSmall()
        {
            Assert.Null(MessageProcessor.ReadUpdateLevel(new byte[12], 11));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is sufficient.
        /// </summary>
        [Test]
        public void TestReadUpdatePositionValid()
        {
            Assert.NotNull(MessageProcessor.ReadUpdatePosition(new byte[16], 16));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is sufficient.
        /// </summary>
        [Test]
        public void TestReadUpdateRotationValid()
        {
            Assert.NotNull(MessageProcessor.ReadUpdateRotation(new byte[8], 8));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is sufficient.
        /// </summary>
        [Test]
        public void TestReadDeleteValid()
        {
            Assert.NotNull(MessageProcessor.ReadDelete(new byte[4], 4));
        }

        /// <summary>
        /// Tests if the <c>ReadInt</c> and <c>WriteInt</c> methods 
        /// cycle.
        /// </summary>
        [Test]
        public void TestReadWriteIntCycles()
        {
            int expected = 77252784;
            byte[] bytes = new byte[4];
            MessageProcessor.WriteInt(expected, bytes, 0);
            int actual = MessageProcessor.ReadInt(bytes, 0);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests if the <c>ReadFloat</c> and <c>WriteFloat</c> methods 
        /// cycle.
        /// </summary>
        [Test]
        public void TestReadWriteFloatCycles()
        {
            float expected = 1.6265f;
            byte[] bytes = new byte[4];
            MessageProcessor.WriteFloat(expected, bytes, 0);
            float actual = MessageProcessor.ReadFloat(bytes, 0);

            Assert.AreEqual(expected, actual, 0.0001f);
        }

        /// <summary>
        /// Tests if the <c>ReadRotationUpdate</c> and <c>WriteRotationUpdate</c>
        /// methods cycle.
        /// </summary>
        [Test]
        public void TestReadWriteRotationUpdateCycles()
        {
            RotationUpdate expected = new RotationUpdate(UpdateType.UpdateRotation, 35.9f, 9);
            byte[] bytes = MessageProcessor.WriteRotationUpdate(expected);

            // The Write method adds a tag byte, but the read method expects it to be removed,
            // so we remove it here.
            byte tag = bytes[0];
            byte[] message = bytes.Skip(1).ToArray<byte>();
            RotationUpdate actual = MessageProcessor.ReadUpdateRotation(message, message.Length);

            // The Equals(object) method of RotationUpdate checks for exact equality of its float parameter,
            // Rotation. However, since this test case does not depend on any float arithmetic, but operates
            // on byte level, we may assume rounding errors have not occurred and as such this check here is 
            // valid.
            Assert.AreEqual(expected, actual);
            Assert.AreEqual((byte)UpdateType.UpdateRotation, tag);
        }

        /// <summary>
        /// Tests if the <c>ReadLevelUpdate</c> and <c>WriteLevelUpdate</c> methods cycle.
        /// </summary>
        [Test]
        public void TestReadWriteLevelUpdateCycles()
        {
            LevelUpdate expected = new LevelUpdate(24, new Vector2(34.5f, 76.2f));
            byte[] bytes = MessageProcessor.WriteLevelUpdate(expected);

            byte tag = bytes[0];
            byte[] message = bytes.Skip(1).ToArray<byte>();
            LevelUpdate actual = MessageProcessor.ReadUpdateLevel(message, message.Length);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual((byte)UpdateType.UpdateLevel, tag);
        }
    }
}