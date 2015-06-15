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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
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
        /// is passed in construct entry.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructEntryTestNull()
        {
            new LevelLoader().ConstructEntry(null);
        }
    }
}
