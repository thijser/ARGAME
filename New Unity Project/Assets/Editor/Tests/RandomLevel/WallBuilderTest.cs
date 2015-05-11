//----------------------------------------------------------------------------
// <copyright file="WallBuilderTest.cs" company="Delft University of Technology">
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

    /// <summary>
    /// Unit tests for the WallBuilder class.
    /// </summary>
    [TestFixture]
    public class WallBuilderTest
    {
        /// <summary>
        /// Tests if the correct exception is thrown
        /// when a null object is passed to the AddRandomWalls method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WallBuildTestSGNull()
        {
            WallBuilder wallbuild = new WallBuilder();
            wallbuild.AddRandomWalls(null);
        }

        /// <summary>
        /// Checks if at least one vertex has been labeled as a wall.
        /// </summary>
        [Test]
        public void WallBuildTestAtLeastOneWall()
        {
            WallBuilder wb = new WallBuilder();
            SquareGraph sg = new SquareGraph(20, 20);
            wb.AddRandomWalls(sg);
            bool check = false;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (sg.GetVertexAtCoordinate(new Coordinate(i, j)).Prop == Property.WALL)
                    {
                        check = true;
                    }
                }
            }

            Assert.IsTrue(check);
        }

        /// <summary>
        /// Checks if at least one vertex has been labeled as empty space.
        /// </summary>
        [Test]
        public void WallBuildTestAtLeastOneEmptySpace()
        {
            WallBuilder builder = new WallBuilder();
            SquareGraph graph = new SquareGraph(20, 20);
            builder.AddRandomWalls(graph);
            bool check = false;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (graph.GetVertexAtCoordinate(new Coordinate(i, j)).Prop == Property.EMPTY)
                    {
                        check = true;
                    }
                }
            }

            Assert.IsTrue(check);
        }
    }
}
