//----------------------------------------------------------------------------
// <copyright file="MarkerTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using NUnit.Framework;
    using System;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="Marker"/> class.
    /// </summary>
    [TestFixture]
    public class MarkerTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if setting the marker ID also updates the ID.
        /// </summary>
        [Test]
        public void TestGetSetIDOnce()
        {
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.ID = 12;
            Assert.AreEqual(12, marker.ID);
        }

        /// <summary>
        /// Tests if setting the marker ID multiple times does not 
        /// change the marker ID after the first set.
        /// </summary>
        [Test]
        public void TestGetSetIDMultipleTimes()
        {
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.ID = 3;
            marker.ID = 12;
            Assert.AreEqual(3, marker.ID);
        }

        /// <summary>
        /// Tests whether calling <c>UpdatePosition</c> with itself as argument causes a position
        /// update that updates the transform of the Marker correctly.
        /// </summary>
        [Test]
        public void TestUpdateParentLocation()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 4;
            parent.LocalPosition = new MarkerPosition(new Vector3(2, 5, 8), Quaternion.Euler(12, 15, 20), DateTime.Now, new Vector3(4, 5, 6), 4);
            parent.RemotePosition = new MarkerPosition(new Vector3(3, 7, 12), Quaternion.Euler(0, 5, 10), DateTime.Now, new Vector3(7, 8, 9), 4);
            parent.UpdatePosition(parent);

            RoughAssert.AreEqual(new Vector3(2, 5, 8), parent.transform.position, 0.01f);
            RoughAssert.AreEqual(Quaternion.Euler(12, 15, 20), parent.transform.rotation, 0.01f);

            // The scale should not be touched: The scale of other markers could be adjusted, 
            // but the parent (level) marker should always have the same scale.
            RoughAssert.AreEqual(Vector3.one, parent.transform.lossyScale, 0.01f);
        }
    }
}