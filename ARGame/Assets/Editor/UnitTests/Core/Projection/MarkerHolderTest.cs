//----------------------------------------------------------------------------
// <copyright file="MarkerHolderTest.cs" company="Delft University of Technology">
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
    public class MarkerHolderTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if calling <c>OnMarkerRegister(null)</c> throws an
        /// <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnMarkerRegisterNull()
        {
            GameObjectFactory.Create<MarkerHolder>().OnMarkerRegister(null);
        }

        /// <summary>
        /// Tests if calling <c>OnMarkerRegister</c> with no parent set will cause 
        /// the argument Marker to not yet become the parent.
        /// </summary>
        [Test]
        public void TestOnMarkerRegisterWithNoParent()
        {
            MarkerHolder updater = GameObjectFactory.Create<MarkerHolder>();
            Marker marker = GameObjectFactory.Create<Marker>();
            marker.ID = 4;
            updater.OnMarkerRegister(new MarkerRegister(marker));
            Assert.Null(updater.Parent);
        }

        /// <summary>
        /// Tests whether the correct exception is thrown when an invalid ID
        /// is requested.
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetMarkerInvalidID()
        {
            MarkerHolder updater = GameObjectFactory.Create<MarkerHolder>();
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
            MarkerHolder updater = GameObjectFactory.Create<MarkerHolder>();
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
            MarkerHolder updater = GameObjectFactory.Create<MarkerHolder>();
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
            GameObjectFactory.Create<MarkerHolder>().OnPositionUpdate(null);
        }

        /// <summary>
        /// Tests whether <c>OnPositionUpdate</c> updates the position of a 
        /// Marker as expected.
        /// </summary>
        [Test]
        public void TestOnPositionUpdateTypical()
        {
            MarkerHolder updater = GameObjectFactory.Create<MarkerHolder>();
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

            MarkerHolder updater = GameObjectFactory.Create<MarkerHolder>();
            updater.OnMarkerRegister(new MarkerRegister(marker));
            updater.OnPositionUpdate(update);

            // Assert the marker data is not changed.
            Assert.AreEqual(6, marker.ID);
            Assert.Null(updater.Parent);
            Assert.Null(marker.LocalPosition);
            Assert.Null(marker.RemotePosition);
        }

        /// <summary>
        /// Tests whether calling <c>OnRotationUpdate(null)</c> throws an
        /// appropriate exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnRotationUpdateNull()
        {
            GameObjectFactory.Create<MarkerHolder>().OnRotationUpdate(null);
        }

        /// <summary>
        /// Tests whether calling <c>OnMarkerRegister(...)</c> with an invalid 
        /// MarkerRegister object throws the correct exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestOnMarkerRegisterWithNullRegister()
        {
            GameObjectFactory.Create<MarkerHolder>().OnMarkerRegister(new MarkerRegister(null));
        }

        /// <summary>
        /// Tests whether calling <c>OnMarkerSeen(null)</c> throws the 
        /// appropriate exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnMarkerSeenNull()
        {
            GameObjectFactory.Create<MarkerHolder>().OnMarkerSeen(null);
        }

        /// <summary>
        /// Tests whether calling <c>OnServerUpdate(null)</c> throws the 
        /// appropriate exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnServerUpdateNull()
        {
            GameObjectFactory.Create<MarkerHolder>().OnServerUpdate(null);
        }
    }
}