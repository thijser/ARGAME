//----------------------------------------------------------------------------
// <copyright file="PositionUpdaterTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using Network;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Unit test for the <see cref="Projection.PositionUpdater"/> class.
    /// </summary>
    [TestFixture]
    public class PositionUpdaterTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if calling <c>OnMarkerRegister(null)</c> throws an
        /// <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnMarkerRegisterNull()
        {
            GameObjectFactory.Create<PositionUpdater>().OnMarkerRegister(null);
        }

        /// <summary>
        /// Tests if calling <c>OnMarkerRegister</c> with no parent set will cause 
        /// the argument Marker to become parent.
        /// </summary>
        [Test]
        public void TestOnMarkerRegisterWithNoParent()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.ID = 4;
            updater.OnMarkerRegister(new MarkerRegister(marker));
            Assert.AreEqual(marker, updater.Parent);
        }

        /// <summary>
        /// Tests if calling <c>OnMarkerRegister</c> with an existing parent will cause
        /// the existing parent to be used as parent, and the parent of the second marker 
        /// should be set to the parent Marker.
        /// </summary>
        [Test]
        public void TestOnMarkerRegisterWithParent()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker parent = GameObjectFactory.Create<Marker>();
            Marker child = GameObjectFactory.Create<Marker>();
            parent.ID = 2;
            child.ID = 4;
            updater.OnMarkerRegister(new MarkerRegister(parent));
            updater.OnMarkerRegister(new MarkerRegister(child));
            Assert.AreEqual(parent, updater.Parent, "PositionUpdater Parent");
            Assert.AreEqual(parent.transform, child.transform.parent, "Marker Hierarchy");
        }

        /// <summary>
        /// Tests whether the correct exception is thrown when an invalid ID
        /// is requested.
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetMarkerInvalidID()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker parent = GameObjectFactory.Create<Marker>();
            updater.OnMarkerRegister(new MarkerRegister(parent));
            updater.GetMarker(2);
        }

        /// <summary>
        /// Tests whether the <c>GetMarker(int)</c> returns the correct marker.
        /// </summary>
        [Test]
        public void TestGetMarkerTypical()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 54;
            updater.OnMarkerRegister(new MarkerRegister(parent));
            Assert.AreEqual(parent, updater.GetMarker(54));
        }

        /// <summary>
        /// Tests whether the <c>GetMarker(int)</c> method succeeds if no markers are 
        /// registered.
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetMarkerWithNoMarkersRegistered()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            updater.GetMarker(0);
        }

        /// <summary>
        /// Tests whether <c>OnPositionUpdate(null)</c> throws 
        /// an <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnPositionUpdateNull()
        {
            GameObjectFactory.Create<PositionUpdater>().OnPositionUpdate(null);
        }

        /// <summary>
        /// Tests whether <c>OnPositionUpdate</c> updates the position of a 
        /// Marker as expected.
        /// </summary>
        [Test]
        public void TestOnPositionUpdateTypical()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.ID = 12;
            PositionUpdate update = new PositionUpdate(UpdateType.UpdatePosition, new Vector2(2, 2), 0, 12);
            updater.OnMarkerRegister(new MarkerRegister(marker));
            updater.OnPositionUpdate(update);
            Assert.AreEqual(new Vector3(2, 0, 2), marker.RemotePosition.Position);
            Assert.AreEqual(Vector3.zero, marker.RemotePosition.Rotation.eulerAngles);
            Assert.AreEqual(12, marker.RemotePosition.ID);
        }

        /// <summary>
        /// Tests whether calling <c>OnPositionUpdate(...)</c> with a update for an
        /// unknown marker does not affect the state of the PositionUpdater.
        /// </summary>
        [Test]
        public void TestOnPositionUpdateNonexistingMarker()
        {
            PositionUpdate update = new PositionUpdate(UpdateType.UpdatePosition, new Vector2(2, 2), 0, 5);
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.ID = 6;
            
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            updater.OnMarkerRegister(new MarkerRegister(marker));
            updater.OnPositionUpdate(update);

            // Assert the marker data is not changed.
            Assert.AreEqual(6, marker.ID);
            Assert.AreEqual(marker, updater.Parent);
            Assert.IsNull(marker.LocalPosition);
            Assert.IsNull(marker.RemotePosition);
        }

        /// <summary>
        /// Tests whether calling <c>UpdateParentPosition(null)</c> throws an 
        /// appropriate exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateParentPositionNull()
        {
            GameObjectFactory.Create<PositionUpdater>().UpdateParentPosition(null);
        }

        /// <summary>
        /// Tests whether calling <c>UpdateParentPosition(...)</c> with a marker that
        /// has <c>LocalPosition</c> set to <c>null</c> throws an appropriate exception. 
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdateParentPositionNullLocalPosition()
        {
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.LocalPosition = null;
            marker.RemotePosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 1);
            
            GameObjectFactory.Create<PositionUpdater>().UpdateParentPosition(marker);
        }

        /// <summary>
        /// Tests whether calling <c>UpdateParentPosition(...)</c> with a marker that
        /// has <c>RemotePosition</c> set to <c>null</c> throws an appropriate exception. 
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdateParentPositionNullRemotePosition()
        {
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.LocalPosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 1);
            marker.RemotePosition = null;

            GameObjectFactory.Create<PositionUpdater>().UpdateParentPosition(marker);
        }

        /// <summary>
        /// Tests whether calling <c>UpdateParentPosition(...)</c> with valid arguments 
        /// updates the position of the marker as expected based on the LocalPosition.
        /// </summary>
        [Test]
        public void TestUpdateParentPositionSetsCorrectLocalPosition()
        {
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.LocalPosition = new MarkerPosition(new Vector3(1, 0, 2), Quaternion.identity, DateTime.Now, Vector3.one, 8);
            marker.RemotePosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 8);

            // We expect the position of the marker to be set to (1, 0, 2) and the rotation to be set to (0, 0, 0)
            GameObjectFactory.Create<PositionUpdater>().UpdateParentPosition(marker);

            Assert.AreEqual(new Vector3(1, 0, 2), marker.transform.position);
            Assert.AreEqual(Vector3.zero, marker.transform.eulerAngles);
        }

        /// <summary>
        /// Tests whether calling <c>UpdateParentPosition(...)</c> with valid arguments 
        /// updates the rotation of the marker as expected based on the LocalPosition.
        /// </summary>
        [Test]
        public void TestUpdateParentPositionSetsCorrectLocalRotation()
        {
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.LocalPosition = new MarkerPosition(
                Vector3.zero, 
                Quaternion.Euler(new Vector3(15, 345, 90)), 
                DateTime.Now, 
                Vector3.one, 
                8);
            marker.RemotePosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 8);

            // We expect the Position to be set to (0, 0, 0) and the rotation to be set to (15, 345, 90)
            GameObjectFactory.Create<PositionUpdater>().UpdateParentPosition(marker);

            Assert.AreEqual(Vector3.zero, marker.transform.position);
            
            Vector3 result = marker.transform.eulerAngles;
            Assert.AreEqual(15, result.x, 0.01);
            Assert.AreEqual(345, result.y, 0.01);
            Assert.AreEqual(90, result.z, 0.01);
        }

        /// <summary>
        /// Tests whether calling <c>UpdateParentPosition(...)</c> with valid arguments 
        /// updates the rotation of the marker as expected based on the RemotePosition.
        /// </summary>
        [Test]
        public void TestUpdateParentPositionSetsCorrectRemoteRotation()
        {
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.RemotePosition = new MarkerPosition(
                Vector3.zero,
                Quaternion.Euler(new Vector3(16, 23, 54)),
                DateTime.Now,
                Vector3.one,
                8);
            marker.LocalPosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 8);

            // The local rotation is 0, so the marker is exactly correct from our perspective. As such, the angle 
            // between our camera's base line and the marker must be equal to 0, regardless of the rotation that the remote
            // server reports (since this is the parent marker). So we expect the Position to be set to (0, 0, 0) and the 
            // rotation to be set to (0, 0, 0).
            GameObjectFactory.Create<PositionUpdater>().UpdateParentPosition(marker);
            
            Assert.AreEqual(Vector3.zero, marker.transform.position);
            Assert.AreEqual(Vector3.zero, marker.transform.eulerAngles);
        }

        /// <summary>
        /// Tests whether calling <c>OnRotationUpdate(null)</c> throws an
        /// appropriate exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnRotationUpdateNull()
        {
            GameObjectFactory.Create<PositionUpdater>().OnRotationUpdate(null);
        }

        /// <summary>
        /// Tests whether calling <c>OnMarkerRegister(...)</c> with an invalid 
        /// MarkerRegister object throws the correct exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestOnMarkerRegisterWithNullRegister()
        {
            GameObjectFactory.Create<PositionUpdater>().OnMarkerRegister(new MarkerRegister(null));
        }

        /// <summary>
        /// Tests whether all object references are set correctly in a hypothetical 
        /// scenario with two markers being seen from both the AR Link and the remote
        /// server.
        /// </summary>
        [Test]
        public void TestOnMarkerSeenWithTwoMarkers()
        {
            Marker markerA = GameObjectFactory.Create<Marker>();
            Marker markerB = GameObjectFactory.Create<Marker>();
            
            markerA.ID = 4;
            markerB.ID = 8;


        }
    }
}