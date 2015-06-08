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
        public void TestReadIntOverflow()
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
        public void TestReadFloatOverflow()
        {
            MessageProcessor.ReadFloat(new byte[16], 13);
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is insufficient.
        /// </summary>
        [Test]
        public void TestReadUpdatePositionInvalidLength()
        {
            Assert.Null(MessageProcessor.ReadUpdatePosition(new byte[16], 15));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is insufficient.
        /// </summary>
        [Test]
        public void TestReadUpdateRotationInvalidLength()
        {
            Assert.Null(MessageProcessor.ReadUpdateRotation(new byte[16], 7));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is insufficient.
        /// </summary>
        [Test]
        public void TestReadDeleteInvalidLength()
        {
            Assert.Null(MessageProcessor.ReadDelete(new byte[16], 3));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is sufficient.
        /// </summary>
        [Test]
        public void TestReadUpdatePositionValid()
        {
            // TODO: Assert field contents are correct.
            Assert.NotNull(MessageProcessor.ReadUpdatePosition(new byte[16], 16));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is sufficient.
        /// </summary>
        [Test]
        public void TestReadUpdateRotationValid()
        {
            // TODO: Assert field contents are correct.
            Assert.NotNull(MessageProcessor.ReadUpdateRotation(new byte[8], 8));
        }

        /// <summary>
        /// Test if correct result is returned when length
        /// is sufficient.
        /// </summary>
        [Test]
        public void TestReadDeleteValid()
        {
            // TODO: Assert field contents are correct.
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
    }
}