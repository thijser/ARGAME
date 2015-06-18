//----------------------------------------------------------------------------
// <copyright file="LevelLoaderTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Level
{
    using System;
    using NUnit.Framework;
    using Projection;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Test class for LevelLoader.
    /// </summary>
    [TestFixture]
    public class LevelLoaderTest : MirrorsUnitTest
    {
        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in create level.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateLevelTestNull()
        {
            new LevelLoader().CreateLevel(null);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in load letters.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadLettersTestNull()
        {
            new LevelLoader().LoadLetters(null);
        }

        /// <summary>
        /// Test if correct exception is thrown when null ref
        /// is passed in construct entry.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructEntryTestNull()
        {
            new LevelLoader().ConstructEntry(null);
        }

        /// <summary>
        /// Tests if constructing an empty level will create 
        /// a properly initialized level GameObject.
        /// </summary>
        [Test]
        public void TestConstructLevelEmpty()
        {
            GameObject level = new LevelLoader().ConstructLevel();

            // The Marker should have a valid RemotePosition set to (0, 0, 0),
            // but the LocalPosition should be set to null.
            Marker marker = level.GetComponent<Marker>();
            Assert.AreEqual(LevelLoader.LevelMarkerID, marker.ID);
            Assert.IsNull(marker.LocalPosition);
            Assert.AreEqual(Vector3.zero, marker.RemotePosition.Position);
        }
    }
}
