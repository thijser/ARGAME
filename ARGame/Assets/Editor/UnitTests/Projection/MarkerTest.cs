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
        /// Tests whether calling <c>UpdatePosition(null)</c> throws an
        /// <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdatePositionNullArgument()
        {
            GameObjectFactory.Create<Marker>().UpdatePosition(null);
        }

        /// <summary>
        /// Tests whether calling <c>UpdatePosition</c> with itself as argument causes a position
        /// update that updates the transform of the Marker correctly.
        /// </summary>
        [Test]
        public void TestUpdatePositionAsParent()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 4;
            parent.LocalPosition = new MarkerPosition(new Vector3(2, 5, 8), Quaternion.Euler(12, 15, 20), DateTime.Now, new Vector3(4, 5, 6), 4);
            parent.RemotePosition = new MarkerPosition(new Vector3(3, 7, 12), Quaternion.Euler(0, 5, 10), DateTime.Now, new Vector3(7, 8, 9), 4);
            parent.UpdatePosition(parent);

            // The parent marker should take over the position and rotation from the Local position.
            RoughAssert.AreEqual(new Vector3(2, 5, 8), parent.transform.position, 0.01f);
            RoughAssert.AreEqual(Quaternion.Euler(12, 15, 20), parent.transform.rotation, 0.01f);
            RoughAssert.AreEqual(new Vector3(4, 5, 6), parent.transform.localScale, 0.01f);
        }

        /// <summary>
        /// Tests whether calling <c>UpdatePosition</c> with itself as argument has no effect
        /// if the marker does not have a remote location set.
        /// </summary>
        [Test]
        public void TestUpdatePositionAsParentWithoutRemoteLocation()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 4;
            parent.LocalPosition = new MarkerPosition(new Vector3(2, 5, 8), Quaternion.Euler(12, 15, 20), DateTime.Now, new Vector3(4, 5, 6), 4);
            parent.UpdatePosition(parent);

            // If the remote camera does not see the marker (remote position == null), then
            // this marker cannot be used as a level marker so this should not affect the transform.
            RoughAssert.AreEqual(Vector3.zero, parent.transform.position, 0.01f);
            RoughAssert.AreEqual(Quaternion.identity, parent.transform.rotation, 0.01f);
            RoughAssert.AreEqual(Vector3.one, parent.transform.lossyScale, 0.01f);
        }

        /// <summary>
        /// Tests if calling <c>UpdatePosition</c> in a scenario with a parent and a child
        /// will cause the child to assume the correct transform if the child is outside the 
        /// view of the local player.
        /// <para>
        /// In this scenario, a board of 70 by 60 was drawn and two markers were added at specific
        /// positions. The local view is rotated by 90 degrees with respect to the remote view, 
        /// causing a shift in the position and rotation of the markers. This example test does not 
        /// test the scale properly, but this is tested in another test case separate from this one 
        /// to be able to isolate issues.
        /// </para>
        /// </summary>
        [Test]
        public void TestUpdatePositionAsChild()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 8;
            parent.RemotePosition = new MarkerPosition(new Vector3(30, 0, 20), Quaternion.identity, DateTime.Now, new Vector3(5, 5, 1), 8);
            parent.LocalPosition = new MarkerPosition(new Vector3(20, 0, 40), Quaternion.Euler(0, 90, 0), DateTime.Now, new Vector3(5, 5, 1), 8);

            Marker child = GameObjectFactory.Create<Marker>();
            child.ID = 3;
            child.RemotePosition = new MarkerPosition(new Vector3(25, 0, 30), Quaternion.Euler(0, -90, 0), DateTime.Now, new Vector3(5, 5, 1), 3);

            MarkerPosition expected = new MarkerPosition(new Vector3(30, 0, 45), Quaternion.identity, DateTime.Now, new Vector3(5, 5, 1), 3);
            child.UpdatePosition(parent);

            Debug.Log("Position: " + child.transform.position);
            Debug.Log("Rotation: " + child.transform.rotation.eulerAngles);
            Debug.Log("Scaling:  " + child.transform.lossyScale);

            RoughAssert.AreEqual(expected.Position, child.transform.position, 0.01f);
            RoughAssert.AreEqual(expected.Rotation, child.transform.rotation, 0.01f);
            RoughAssert.AreEqual(expected.Scale, child.transform.lossyScale, 0.01f);
        }
    }
}