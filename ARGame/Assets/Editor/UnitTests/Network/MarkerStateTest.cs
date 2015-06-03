//----------------------------------------------------------------------------
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
        /// is passed in constructor.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullTest()
        {
            new MarkerState(null, new GameObject("Dummy"));
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in constructor, again.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullTest2()
        {
            new MarkerState(new PositionUpdate(UpdateType.Ping, new Vector2(0, 0), 0f, 1), null);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in update.
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullTest()
        {
            GameObject parent = new GameObject("Dummy");
            GameObject child = new GameObject("NumberText", typeof(TextMesh));
            child.transform.parent = parent.transform;
            MarkerState ms = new MarkerState(new PositionUpdate(UpdateType.Ping, new Vector2(0, 0), 0f, 1), parent);
            ms.Update(null);
        }

        /// <summary>
        /// Test if correct exception is thrown when invalid
        /// args are used.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorInvalidArgsTest()
        {
            GameObject parent = new GameObject("Dummy");
            MarkerState ms = new MarkerState(new PositionUpdate(UpdateType.Ping, new Vector2(0, 0), 0f, 1), parent);
        }

        /// <summary>
        /// Test if correct exception is thrown when invalid
        /// args are used, again.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorInvalidArgsTestPartTwo()
        {
            GameObject parent = new GameObject("Dummy");
            GameObject child = new GameObject("NumberText");
            MarkerState ms = new MarkerState(new PositionUpdate(UpdateType.Ping, new Vector2(0, 0), 0f, 1), parent);
        }

        /// <summary>
        /// Test if correct behaviour is executed with an updateposition.
        /// </summary>
        [Test]
        public void UpdateValidTest()
        {
            GameObject parent = new GameObject("Dummy");
            GameObject child = new GameObject("NumberText", typeof(TextMesh));
            child.transform.parent = parent.transform;
            parent.transform.position = Vector3.zero;
            MarkerState ms = new MarkerState(new PositionUpdate(UpdateType.Ping, new Vector2(0f, 0f), 0f, 1), parent);
            ms.Update(new PositionUpdate(UpdateType.UpdatePosition, new Vector2(5f, 5f), 0f, 1));
            Assert.True(Vector3.zero != ms.Object.transform.position);
            Assert.True(ms.Object.activeSelf);
        }

        /// <summary>
        /// Test if correct behaviour is executed with an updateposition.
        /// </summary>
        [Test]
        public void UpdateValidTest2()
        {
            GameObject parent = new GameObject("Dummy");
            GameObject child = new GameObject("NumberText", typeof(TextMesh));
            child.transform.parent = parent.transform;
            parent.transform.position = Vector3.zero;
            MarkerState ms = new MarkerState(new PositionUpdate(UpdateType.Ping, new Vector2(0f, 0f), 0f, 1), parent);
            ms.Update(new PositionUpdate(UpdateType.DeletePosition, new Vector2(5f, 5f), 0f, 1));
            Assert.False(ms.Object.activeSelf);
        }
    }
}
