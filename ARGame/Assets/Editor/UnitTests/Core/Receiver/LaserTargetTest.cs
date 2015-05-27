//----------------------------------------------------------------------------
// <copyright file="LaserTargetTest.cs" company="Delft University of Technology">
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
    /// A test class for the LaserTarget object.
    /// </summary>
    [TestFixture]
    public class LaserTargetTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the laser target is not opening when 
        /// no laser hits it.
        /// </summary>
        [Test]
        public static void NoLaserHitTest()
        {
            LaserTarget lt = CreateLaserTarget();
            Assert.False(lt.IsOpening);
        }

        /// <summary>
        /// Tests if crystal is opening when a valid laser beam Hit it.
        /// </summary>
        [Test]
        public static void LaserHitTest()
        {
            GameObject gameObject = new GameObject("LaserTarget", typeof(LaserTarget));
            gameObject.AddComponent<Animator>();
            gameObject.GetComponent<LaserTarget>().OnLaserHit(null, new HitEventArgs(CreateTestBeam(), Vector3.zero, Vector3.forward, CreateAndGate()));
            gameObject.GetComponent<LaserTarget>().LateUpdate();
            Assert.True(gameObject.GetComponent<LaserTarget>().IsOpening);
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
            LaserTarget lt = CreateLaserTarget();
            lt.OnLaserHit(null, new HitEventArgs());
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
            LaserTarget lt = (LaserTarget)CreateLaserTarget();
            lt.OnLaserHit(null, null);
        }
    }
}
