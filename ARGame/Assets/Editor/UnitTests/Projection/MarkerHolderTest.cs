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
    using System;
    using System.Collections.Generic;
    using Network;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="Projection.PositionUpdater"/> class.
    /// </summary>
    [TestFixture]
    public class MarkerHolderTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests whether the <c>GetMarker(int)</c> method succeeds if no markers are
        /// registered.
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetMarkerWithNoMarkersRegistered()
        {
            LocalMarkerHolder updater = GameObjectFactory.Create<LocalMarkerHolder>();
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
            GameObjectFactory.Create<LocalMarkerHolder>().OnPositionUpdate(null);
        }

        /// <summary>
        /// Tests whether <c>OnPositionUpdate</c> updates the position of a
        /// Marker as expected.
        /// </summary>
        [Test]
        public void TestOnPositionUpdateTypical()
        {
            LocalMarkerHolder updater = GameObjectFactory.Create<LocalMarkerHolder>();
            LocalMarker marker = GameObjectFactory.Create<LocalMarker>();
            marker.Id = 12;
            PositionUpdate update = new PositionUpdate(UpdateType.UpdatePosition, new Vector2(2, 2), 34, 12);
            updater.AddMarker(marker);
            updater.OnPositionUpdate(update);
            
            RoughAssert.AreEqual(new Vector3(16, 0, -16), marker.RemotePosition.Position, 0.01f);
            RoughAssert.AreEqual(new Vector3(0, 34, 0), marker.RemotePosition.Rotation.eulerAngles, 0.01f);
            Assert.AreEqual(12, marker.RemotePosition.ID);
        }

        /// <summary>
        /// Tests whether calling <c>OnPositionUpdate(...)</c> with a serverUpdate for an
        /// unknown marker does not affect the state of the PositionUpdater.
        /// </summary>
        [Test]
        public void TestOnPositionUpdateNonexistingMarker()
        {
            PositionUpdate update = new PositionUpdate(UpdateType.UpdatePosition, new Vector2(2, 2), 0, 5);
            LocalMarker marker = GameObjectFactory.Create<LocalMarker>();
            marker.Id = 6;

            LocalMarkerHolder updater = GameObjectFactory.Create<LocalMarkerHolder>();
            updater.AddMarker(marker);
            updater.OnPositionUpdate(update);

            // Assert the marker data is not changed.
            Assert.AreEqual(6, marker.Id);
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
            GameObjectFactory.Create<LocalMarkerHolder>().OnRotationUpdate(null);
        }

        /// <summary>
        /// Tests whether calling <c>OnMarkerRegister(...)</c> with an invalid
        /// MarkerRegister object throws the correct exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestOnMarkerRegisterWithNullRegister()
        {
            GameObjectFactory.Create<LocalMarkerHolder>().AddMarker(null);
        }

        /// <summary>
        /// Tests whether calling <c>OnMarkerSeen(null)</c> throws the
        /// appropriate exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOnMarkerSeenNull()
        {
            GameObjectFactory.Create<LocalMarkerHolder>().OnMarkerSeen(null);
        }

        /// <summary>
        /// Tests whether calling <c>SelectParent(null)</c> throws the
        /// appropriate exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelectParentNull()
        {
            GameObjectFactory.Create<LocalMarkerHolder>().SelectParent(null);
        }

        /// <summary>
        /// Tests whether <c>OnRotationUpdate</c> updates the position of a
        /// Marker as expected.
        /// </summary>
        [Test]
        public void TestOnRotationUpdateTypical()
        {
            LocalMarkerHolder updater = GameObjectFactory.Create<LocalMarkerHolder>();
            LocalMarker marker = GameObjectFactory.Create<LocalMarker>();
            marker.Id = 12;
            RotationUpdate update = new RotationUpdate(UpdateType.UpdateRotation, 180, 12);
            updater.AddMarker(marker);
            updater.OnRotationUpdate(update);
            Assert.AreEqual(180, marker.ObjectRotation);
        }
    }
}
