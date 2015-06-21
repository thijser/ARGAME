﻿//----------------------------------------------------------------------------
// <copyright file="MarkerStateTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Test class for MarkerState.
    /// </summary>
    [TestFixture]
    public class MarkerStateTest : MirrorsUnitTest
    {
        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in constructor, again.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullTest2()
        {
            new MarkerState(1, null);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in serverUpdate.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullTest()
        {
            GameObject parent = new GameObject("Dummy");
            GameObject child = new GameObject("NumberText", typeof(TextMesh));
            child.transform.parent = parent.transform;
            MarkerState ms = new MarkerState(1, parent);
            ms.UpdatePosition(null);
        }

        /// <summary>
        /// Test if correct behavior is executed with a <see cref="PositionUpdate"/>.
        /// </summary>
        [Test]
        public void UpdateValidTest()
        {
            GameObject parent = new GameObject("Dummy");
            GameObject child = new GameObject("NumberText", typeof(TextMesh));
            child.transform.parent = parent.transform;
            parent.transform.position = Vector3.zero;
            MarkerState ms = new MarkerState(1, parent);
            ms.UpdatePosition(new PositionUpdate(UpdateType.UpdatePosition, new Vector2(5f, 5f), 0f, 1));
            Assert.True(Vector3.zero != ms.Object.transform.position);
            Assert.True(ms.Object.activeSelf);
        }

        /// <summary>
        /// Test if correct behavior is executed with a <see cref="PositionUpdate"/>.
        /// </summary>
        [Test]
        public void UpdateValidTest2()
        {
            GameObject parent = new GameObject("Dummy");
            GameObject child = new GameObject("NumberText", typeof(TextMesh));
            child.transform.parent = parent.transform;
            parent.transform.position = Vector3.zero;
            MarkerState ms = new MarkerState(1, parent);
            ms.UpdatePosition(new PositionUpdate(UpdateType.DeletePosition, new Vector2(5f, 5f), 0f, 1));
            Assert.False(ms.Object.activeSelf);
            ms.Object.SetActive(true);
        }
    }
}
