//----------------------------------------------------------------------------
// <copyright file="LevelUpdateTest.cs" company="Delft University of Technology">
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
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="LevelUpdate"/> class.
    /// <para>
    /// This NUnit test class tests the <see cref="LevelUpdate"/> constructor
    /// and tests if input arguments have the correct outcomes using a one-by-one
    /// domain testing strategy.
    /// </para>
    /// </summary>
    [TestFixture]
    public class LevelUpdateTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the <see cref="LevelUpdate"/> constructor
        /// initializes a correct <see cref="LevelUpdate"/>
        /// instance.
        /// </summary>
        [Test]
        public void TestConstructorTypical()
        {
            LevelUpdate update = new LevelUpdate(5, new Vector2(3, 4));
            Assert.AreEqual(5, update.NextLevelIndex);
            Assert.AreEqual(new Vector2(3, 4), update.Size);
        }

        /// <summary>
        /// Tests if providing an out of range width to the 
        /// <see cref="LevelUpdate"/> constructor results in the
        /// appropriate exception.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected Exception.</exception>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructorSizeInvalidX()
        {
            new LevelUpdate(5, new Vector2(0, 4));
        }
        
        /// <summary>
        /// Tests if providing the lowest permissible width to
        /// the <see cref="LevelUpdate"/> constructor initializes
        /// a correct <see cref="LevelUpdate"/> instance.
        /// </summary>
        [Test]
        public void TestConstructorSizeLowestX()
        {
            LevelUpdate update = new LevelUpdate(5, new Vector2(1, 4));
            Assert.AreEqual(1, update.Size.x);
        }

        /// <summary>
        /// Tests if providing an out of range height to the 
        /// <see cref="LevelUpdate"/> constructor results in the
        /// appropriate exception.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected Exception.</exception>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructorSizeInvalidY()
        {
            new LevelUpdate(5, new Vector2(3, 0));
        }

        /// <summary>
        /// Tests if providing the lowest permissible height to
        /// the <see cref="LevelUpdate"/> constructor initializes
        /// a correct <see cref="LevelUpdate"/> instance.
        /// </summary>
        [Test]
        public void TestConstructorSizeLowestY()
        {
            LevelUpdate update = new LevelUpdate(5, new Vector2(3, 1));
            Assert.AreEqual(1, update.Size.y);
        }

        /// <summary>
        /// Tests if providing an out of range level index to the 
        /// <see cref="LevelUpdate"/> constructor results in the
        /// appropriate exception.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected Exception.</exception>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructorInvalidLevel()
        {
            new LevelUpdate(-1, new Vector2(3, 4));
        }

        /// <summary>
        /// Tests if providing the lowest permissible level index to
        /// the <see cref="LevelUpdate"/> constructor initializes
        /// a correct <see cref="LevelUpdate"/> instance.
        /// </summary>
        [Test]
        public void TestConstructorSizeLowestLevel()
        {
            LevelUpdate update = new LevelUpdate(0, new Vector2(3, 4));
            Assert.AreEqual(0, update.NextLevelIndex);
        }

    }
}