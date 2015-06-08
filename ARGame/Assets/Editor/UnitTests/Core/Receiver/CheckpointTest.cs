//----------------------------------------------------------------------------
// <copyright file="CheckpointTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Receiver
{
    using System;
    using Core.Emitter;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// A test class for the Checkpoint object.
    /// </summary>
    [TestFixture]
    public class CheckpointTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the values are set correctly
        /// at the start of Checkpoint.
        /// </summary>
        [Test]
        public static void StartTest()
        {
            Checkpoint c = Create<Checkpoint>();
            c.Start();
            Assert.False(c.Hit);
            Assert.NotNull(c.PassThroughEmitter);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// an invalid HitEventArgs object is used in the 
        /// OnLaserHit object.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public static void LaserHitInvalidTest()
        {
            Checkpoint c = Create<Checkpoint>();
            c.OnLaserHit(null, new HitEventArgs());
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null HitEventArgs object is used in the 
        /// OnLaserHit object.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public static void LaserHitNullTest()
        {
            Checkpoint c = Create<Checkpoint>();
            c.OnLaserHit(null, null);
        }

        /// <summary>
        /// Tests for correct behavior after being hit 
        /// by a valid laser beam.
        /// </summary>
        [Test]
        public static void LaserHitRegularTest()
        {
            Checkpoint c = Create<Checkpoint>();
            c.Start();
            c.OnLaserHit(null, GameObjectFactory.CreateTestHit());
            Assert.True(c.Hit);
            c.Reset();
            Assert.False(c.Hit);
        }
    }
}
