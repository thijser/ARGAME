//----------------------------------------------------------------------------
// <copyright file="RandomLevelGeneratorTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace RandomLevel
{
    using System;
    using NUnit.Framework;
    using TestUtilities;

    /// <summary>
    /// Unit tests for the RandomLevelGenerator class.
    /// </summary>
    [TestFixture]
    public class RandomLevelGeneratorTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the correct exception is thrown once the constructor
        /// is called with an amount of rows less than ten.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateRLGRowsLessThanTen()
        {
            new RandomLevelGenerator(9, 10);
        }

        /// <summary>
        /// Tests if the correct exception is thrown once the constructor
        /// is called with an amount of columns less than ten.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateRLGColsLessThanTen()
        {
            new RandomLevelGenerator(10, 9);
        }

        /// <summary>
        /// Tests if the created random map is not null if the constructor
        /// is called with valid parameters.
        /// </summary>
        [Test]
        public void CreateRLGResultNonZeroTest()
        {
            RandomLevelGenerator generator = new RandomLevelGenerator(10, 10);
            Assert.NotNull(generator.Graph);
        }
    }
}
