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
        /// Tests whether calling <c>UpdatePosition</c> with itself as argument throws
        /// an Exception if the marker does not have a remote location set.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdatePositionAsParentWithoutRemoteLocation()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 4;
            parent.LocalPosition = new MarkerPosition(new Vector3(2, 5, 8), Quaternion.Euler(12, 15, 20), DateTime.Now, new Vector3(4, 5, 6), 4);
            parent.UpdatePosition(parent);
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
        public void TestUpdatePositionAsChildSimpleCase()
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

            RoughAssert.AreEqual(expected.Position, child.transform.position, 0.01f);
            RoughAssert.AreEqual(expected.Rotation, child.transform.rotation, 0.01f);
            RoughAssert.AreEqual(expected.Scale, child.transform.lossyScale, 0.01f);
        }

        /// <summary>
        /// Tests if calling <c>UpdatePosition</c> in a scenario where the level marker's remote and 
        /// local positions are equal, will result in the remote and local positions of a child marker
        /// to be equal as well.
        /// </summary>
        [Test]
        public void TestUpdatePositionAsChildIdenticalCase()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 8;
            parent.RemotePosition = new MarkerPosition(new Vector3(30, 0, 20), Quaternion.Euler(0, -23, 0), DateTime.Now, new Vector3(8, 5, 1), 8);
            parent.LocalPosition = parent.RemotePosition;

            Marker child = GameObjectFactory.Create<Marker>();
            child.ID = 3;
            MarkerPosition expected = new MarkerPosition(new Vector3(25, 0, 30), Quaternion.Euler(0, -87, 0), DateTime.Now, new Vector3(6, 10, 1), 3);
            child.RemotePosition = expected;

            child.UpdatePosition(parent);

            RoughAssert.AreEqual(expected.Position, child.transform.position, 0.01f);
            RoughAssert.AreEqual(expected.Rotation, child.transform.rotation, 0.01f);
            RoughAssert.AreEqual(expected.Scale, child.transform.lossyScale, 0.01f);
        }

        /// <summary>
        /// Tests whether calling <c>UpdatePosition</c> for a rotation around the x-axis 
        /// results in the correct coordinates for the child marker.
        /// </summary>
        [Test]
        public void TestUpdatePositionRotateAroundXAxis()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 10;
            parent.RemotePosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 10);
            parent.LocalPosition = new MarkerPosition(Vector3.zero, Quaternion.Euler(-90, 0, 0), DateTime.Now, Vector3.one, 10);

            Marker child = GameObjectFactory.Create<Marker>();
            child.ID = 7;
            child.RemotePosition = new MarkerPosition(new Vector3(20, 0, 10), Quaternion.identity, DateTime.Now, Vector3.one, 7);
            child.UpdatePosition(parent);

            MarkerPosition expected = new MarkerPosition(new Vector3(20, 10, 0), Quaternion.Euler(-90, 0, 0), DateTime.Now, Vector3.one, 7);
            
            RoughAssert.AreEqual(expected.Position, child.transform.localPosition, 0.01f);
            RoughAssert.AreEqual(expected.Rotation, child.transform.localRotation, 0.01f);
            RoughAssert.AreEqual(expected.Scale, child.transform.localScale, 0.01f);
        }

        /// <summary>
        /// Tests whether calling <c>UpdatePosition</c> for a rotation around the x-axis 
        /// as well as a translation results in the correct coordinates for the child marker.
        /// </summary>
        [Test]
        public void TestUpdatePositionWithRotationAndTranslation()
        {
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 10;
            parent.RemotePosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 10);
            parent.LocalPosition = new MarkerPosition(new Vector3(10, 30, 20), Quaternion.Euler(-90, 0, 0), DateTime.Now, Vector3.one, 10);

            Marker child = GameObjectFactory.Create<Marker>();
            child.ID = 7;
            child.RemotePosition = new MarkerPosition(new Vector3(20, 0, 10), Quaternion.identity, DateTime.Now, Vector3.one, 7);
            child.UpdatePosition(parent);

            MarkerPosition expected = new MarkerPosition(new Vector3(30, 40, 20), Quaternion.Euler(-90, 0, 0), DateTime.Now, Vector3.one, 7);

            RoughAssert.AreEqual(expected.Position, child.transform.localPosition, 0.01f);
            RoughAssert.AreEqual(expected.Rotation, child.transform.localRotation, 0.01f);
            RoughAssert.AreEqual(expected.Scale, child.transform.localScale, 0.01f);
        }

        /// <summary>
        /// Tests whether calling <c>UpdatePosition</c> for a translation 
        /// as well as a scaling results in the correct coordinates for the child marker.
        /// <para>
        /// To assert the scaling and translation do not affect the rotation, an arbitrary 
        /// rotation is chosen as the plane over which the translation and rotation happen.
        /// </para>
        /// </summary>
        [Test]
        public void TestUpdatePositionWithTranslationAndScale()
        {
            Quaternion rotation = Quaternion.Euler(87, 23, 15);
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 10;
            parent.RemotePosition = new MarkerPosition(new Vector3(40, 40, 0), rotation, DateTime.Now, Vector3.one, 10);
            parent.LocalPosition = new MarkerPosition(new Vector3(-40, -20, 0), rotation, DateTime.Now, Vector3.one / 2, 10);

            Marker child = GameObjectFactory.Create<Marker>();
            child.ID = 7;
            child.RemotePosition = new MarkerPosition(new Vector3(35, 20, 0), rotation, DateTime.Now, Vector3.one, 7);
            child.UpdatePosition(parent);

            MarkerPosition expected = new MarkerPosition(new Vector3(-42.5f, -30, 0), rotation, DateTime.Now, Vector3.one / 2, 7);

            RoughAssert.AreEqual(expected.Position, child.transform.localPosition, 0.01f);
            RoughAssert.AreEqual(expected.Rotation, child.transform.localRotation, 0.01f);
            RoughAssert.AreEqual(expected.Scale, child.transform.localScale, 0.01f);
        }

        /// <summary>
        /// Tests if the <c>UpdatePosition(...)</c> method sets the correct position
        /// based on real data retrieved from the Meta glasses and the server.
        /// <para>
        /// This data was retrieved by monitoring the local and remote positions as reported 
        /// by the IARLink and Mirror Server implementations, respectively.
        /// </para>
        /// </summary>
        [Test]
        public void TestUpdatePositionRealData()
        {
            float scale = -0.005f;
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 4;
            parent.RemotePosition = new MarkerPosition(
                new Vector3(32.4f, 0, 61.5f), 
                Quaternion.identity, 
                DateTime.Now, 
                Vector3.one, 
                4);
            parent.LocalPosition = new MarkerPosition(
                new Vector3(0, -0.3f, 0.6f), 
                Quaternion.Euler(356.6f, 275.4f, 11.4f), 
                DateTime.Now, 
                scale * Vector3.one, 
                4);

            Marker child = GameObjectFactory.Create<Marker>();
            child.ID = 5;
            child.RemotePosition = new MarkerPosition(
                new Vector3(28.4f, 0, 44.6f), 
                Quaternion.Euler(0, -90f, -180f), 
                DateTime.Now, 
                Vector3.one, 
                5);
            child.UpdatePosition(parent);

            MarkerPosition expected = new MarkerPosition(
                new Vector3(-0.1f, -0.3f, 0.6f),
                Quaternion.Euler(349.3f, 7.4f, 357.2f),
                DateTime.Now,
                scale * Vector3.one,
                5);

            RoughAssert.AreEqual(expected.Position, child.transform.localPosition, 0.1f);
            RoughAssert.AreEqual(expected.Rotation, child.transform.localRotation, 36f);
            RoughAssert.AreEqual(expected.Scale, child.transform.localScale, 0.1f);
        }
    }
}