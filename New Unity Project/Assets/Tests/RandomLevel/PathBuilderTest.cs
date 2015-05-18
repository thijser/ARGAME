//----------------------------------------------------------------------------
// <copyright file="PathBuilderTest.cs" company="Delft University of Technology">
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
    /// Unit test for the <see cref="RandomLevel.PathBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class PathBuilderTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the correct exception is thrown when a null reference
        /// is used in the constructor as SquareGraph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSquareGraphTest()
        {
            new PathBuilder(null, new Coordinate(0, 0));
        }

        /// <summary>
        /// Tests if the correct exception is thrown when a null reference
        /// is used in the constructor as Coordinate.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullCoordinateTest()
        {
            new PathBuilder(new SquareGraph(1, 1), null);
        }
    }
}
