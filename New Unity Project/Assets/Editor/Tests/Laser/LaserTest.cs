//----------------------------------------------------------------------------
// <copyright file="LaserTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser
{
    using System;
    using NUnit.Framework;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="Laser.Laser"/> class.
    /// </summary>
    [TestFixture]
    public class LaserTest
    {
        /// <summary>
        /// Creates a LaserEmitter instance to create Laser segments with.
        /// </summary>
        /// <returns>The LaserEmitter.</returns>
        public static LaserEmitter CreateEmitter()
        {
            GameObject gameObject = new GameObject("Emitter", typeof(LaserEmitter));
            return gameObject.GetComponent<LaserEmitter>();
        }

        /// <summary>
        /// Creates a Laser beam with the given properties.
        /// </summary>
        /// <returns>The created Laser.</returns>
        /// <param name="origin">The origin.</param>
        /// <param name="direction">The direction.</param>
        public static Laser CreateLaser(Vector3 origin, Vector3 direction)
        {
            Laser laser = new Laser(origin, direction, CreateEmitter());
            laser.Create();
            return laser;
        }

        /// <summary>
        /// Creates a GameObject with a BoxCollider at the given position.
        /// </summary>
        /// <param name="position">The position of the BoxCollider.</param>
        /// <param name="size">The size of the BoxCollider.</param>
        /// <returns>The created BoxCollider.</returns>
        public static BoxCollider CreateCollider(Vector3 position, float size)
        {
            GameObject gameObject = new GameObject("Collider", typeof(BoxCollider));
            gameObject.transform.position = position;
            gameObject.GetComponent<BoxCollider>().size = new Vector3(size, size, size);
            return gameObject.GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Tests the basic properties of the Laser and the <c>CreateLaser</c> method
        /// used by other test cases.
        /// </summary>
        [Test]
        public void CreateLaserTest()
        {
            Vector3 origin = new Vector3(0, 1, 2);
            Vector3 direction = new Vector3(3, 4, 5);
            Laser laser = CreateLaser(origin, direction);

            Assert.AreEqual(origin, laser.Origin);
            Assert.AreEqual(direction, laser.Direction);
        }

        [Test]
        public void FindReceiverWithNoReceiverTest()
        {
            HitEventArgs args;
            Laser laser = CreateLaser(Vector3.zero, Vector3.forward);
            ILaserReceiver receiver = laser.FindReceiver(out args);

            Assert.IsNull(receiver);
            Assert.IsNull(args);
        }

        /// <summary>
        /// Tests if the <c>Extend</c> method produces a Laser segment with the 
        /// correct properties.
        /// </summary>
        [Test]
        public void ExtendTest()
        {
            Laser laser = CreateLaser(Vector3.zero, Vector3.forward);
            Laser extension = laser.Extend(Vector3.zero, Vector3.left);

            Assert.AreEqual(Vector3.zero, extension.Origin);
            Assert.AreEqual(Vector3.left, extension.Direction);
            Assert.AreSame(laser.Emitter, extension.Emitter);
        }
    }
}
