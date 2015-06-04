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
        public void PosUpdateNullTest()
        {
            MessageProcessor.ReadUpdatePosition(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RotUpdateNullTest()
        {
            MessageProcessor.ReadUpdateRotation(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelNullTest()
        {
            MessageProcessor.ReadDelete(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IntNullTest()
        {
            MessageProcessor.ReadInt(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FloatNullTest()
        {
            MessageProcessor.ReadFloat(null, 0);
        }

        /// <summary>
        /// Test if correct exception is thrown when length
        /// is insufficient.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PosUpdateNullTest()
        {
            MessageProcessor.ReadUpdatePosition(new byte[16], 0);
        }
    }
}