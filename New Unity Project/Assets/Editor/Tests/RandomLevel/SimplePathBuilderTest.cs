//----------------------------------------------------------------------------
// <copyright file="SimplePathBuilderTest.cs" company="Delft University of Technology">
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
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="SimplePathBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class SimplePathBuilderTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if passing null into the SimplePathBuilder constructor throws 
        /// an ArgumentNullException.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPassNullIntoConstructor()
        {
            new SimplePathBuilder(null);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the first
        /// parameter of the PathFromToRow-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPathFromToRowWithInvalidInitialRow()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(20, 10));
            builder.PathFromToRow(20, 5, 5);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the second
        /// parameter of the PathFromToRow-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPathFromToRowWithInvalidEndRow()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(20, 10));
            builder.PathFromToRow(5, 20, 5);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the third
        /// parameter of the PathFromToRow-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPathFromToRowWithInvalidColumn()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(20, 10));
            builder.PathFromToRow(10, 10, 10);
        }

        /// <summary>
        /// Tests if the PathFromToRow method labels vertices correctly,
        /// given that the parameters are valid with respect to this 
        /// square graph.
        /// </summary>
        [Test]
        public void TestPathFromToRowTypicalCase()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(20, 10));
            builder.PathFromToRow(4, 6, 5);
            Assert.AreEqual(Property.PARTOFPATH, builder.Graph.GetVertexAtCoordinate(new Coordinate(5, 5)).Property);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the first
        /// parameter of the PathFromToCol-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPathFromToColWithInvalidRow()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(10, 20));
            builder.PathFromToCol(20, 5, 5);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the second
        /// parameter of the PathFromToCol-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPathFromToColWithInvalidInitialColumn()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(10, 20));
            builder.PathFromToCol(5, 20, 10);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the third
        /// parameter of the PathFromToCol-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPathFromToColWithInvalidEndColumn()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(10, 20));
            builder.PathFromToCol(5, 15, 20);
        }

        /// <summary>
        /// Tests if the PathFromToCol method labels vertices correctly,
        /// given that the parameters are valid with respect to this 
        /// square graph.
        /// </summary>
        [Test]
        public void TestPathFromToColTypicalCase()
        {
            SimplePathBuilder builder = new SimplePathBuilder(new SquareGraph(20, 20));
            builder.PathFromToCol(5, 4, 6);
            Assert.AreEqual(Property.PARTOFPATH, builder.Graph.GetVertexAtCoordinate(new Coordinate(5, 5)).Property);
        }
    }
}