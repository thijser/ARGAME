﻿//----------------------------------------------------------------------------
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

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetMarkerInvalidID()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker parent = GameObjectFactory.Create<Marker>();
            updater.OnMarkerRegister(new MarkerRegister(parent));
            updater.GetMarker(2);
        }

        [Test]
        public void TestGetMarkerTypical()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 54;
            updater.OnMarkerRegister(new MarkerRegister(parent));
            Assert.AreEqual(parent, updater.GetMarker(54));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetMarkerWithNoMarkersRegistered()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            updater.GetMarker(0);
        }

        [Test]
        public void TestOnPositionUpdate()
        {
            PositionUpdater updater = GameObjectFactory.Create<PositionUpdater>();
            Marker parent = GameObjectFactory.Create<Marker>();
            parent.ID = 54;
        }
    }
}