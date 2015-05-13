//----------------------------------------------------------------------------
// <copyright file="MirrorTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser.Receiver
{
    using System;
    using System.Collections.ObjectModel;
    using NUnit.Framework;
    using UnityEngine;
    using Laser.Emitter;

    /// <summary>
    /// Unit test class for the <see cref="Mirror"/> class.
    /// </summary>
    [TestFixture]
    internal class MirrorTest
    {
        /// <summary>
        /// Creates an emitter.
        /// </summary>
        /// <returns>The emitter.</returns>
        public static LaserEmitter CreateEmitter()
        {
            GameObject gameObject = new GameObject("Emitter", typeof(LaserEmitter));
            return gameObject.GetComponent<LaserEmitter>();
        }

        /// <summary>
        /// Creates a Mirror GameObject.
        /// </summary>
        /// <returns>The created Mirror</returns>
        public static Mirror CreateMirror()
        {
            GameObject obj = new GameObject("Mirror", typeof(Mirror));
            return obj.GetComponent<Mirror>();
        }

        /// <summary>
        /// Tests if two Vector3 objects are equal.
        /// </summary>
        /// <param name="expected">The expected Vector3.</param>
        /// <param name="actual">The actual Vector3.</param>
        /// <param name="margin">The margin for rounding errors.</param>
        public static void AreEqual(Vector3 expected, Vector3 actual, float margin)
        {
            Assert.AreEqual(expected.x, actual.x, margin);
            Assert.AreEqual(expected.y, actual.y, margin);
            Assert.AreEqual(expected.z, actual.z, margin);
        }

        /// <summary>
        /// Tests if calling <c>OnLaserHit(this, null)</c> throws an
        /// ArgumentNullException.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullHitArgsTest() 
        {
            CreateMirror().OnLaserHit(this, null);
        }

        /// <summary>
        /// Tests if calling <c>CreateReflection(null, Vector3.zero)</c>
        /// throws an ArgumentNullException.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullLaserBeamTest() 
        {
            Mirror.CreateReflection(null, Vector3.zero);
        }

        /// <summary>
        /// Tests if a perpendicular laser beam is reflected in the 
        /// opposite direction.
        /// </summary>
        [Test]
        public void PerpendicularReflectionTest()
        {
            LaserBeam original = new LaserBeam(Vector3.zero, Vector3.forward, CreateEmitter());
            LaserBeam reflection = Mirror.CreateReflection(original, Vector3.back);
            AreEqual(Vector3.back, reflection.Direction, 0.01f);
        }
    }
}