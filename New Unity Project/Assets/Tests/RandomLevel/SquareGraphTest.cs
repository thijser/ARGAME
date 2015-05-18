//----------------------------------------------------------------------------
// <copyright file="SquareGraphTest.cs" company="Delft University of Technology">
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
    /// Unit test for the <see cref="SquareGraph"/> class. 
    /// </summary>
    [TestFixture]
    public class SquareGraphTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if creating a graph without any rows causes 
        /// an <c>ArgumentOutOfRangeException</c> to be thrown.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructorZeroRows()
        {
            new SquareGraph(0, 20);
        }

        /// <summary>
        /// Tests if a graph with a single row is valid.
        /// </summary>
        [Test]
        public void TestConstructorMinimalRows()
        {
            SquareGraph graph = new SquareGraph(1, 20);
            Assert.AreEqual(1, graph.Maxrow);
        }

        /// <summary>
        /// Tests if creating a graph without any columns causes 
        /// an <c>ArgumentOutOfRangeException</c> to be thrown.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructorZeroColumns()
        {
            new SquareGraph(20, 0);
        }

        /// <summary>
        /// Tests if a graph with a single column is valid.
        /// </summary>
        [Test]
        public void TestConstructorMinimalColumns()
        {
            SquareGraph graph = new SquareGraph(20, 1);
            Assert.AreEqual(1, graph.Maxcol);
        }

        /// <summary>
        /// Tests if creating a graph with typical values for rows and columns
        /// results in a correctly sized graph.
        /// </summary>
        [Test]
        public void TestConstructorTypicalCase()
        {
            SquareGraph sg = new SquareGraph(10, 20);
            Assert.AreEqual(10, sg.Maxrow);
            Assert.AreEqual(20, sg.Maxcol);
        }

        /// <summary>
        /// Tests if an <c>ArgumentNullException</c> is thrown when a null
        /// coordinate is passed as a parameter in the IsValid-method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestIsValidWithNullArgument()
        {
            SquareGraph sg = new SquareGraph(20, 20);
            sg.IsValid(null);
        }

        /// <summary>
        /// Tests if the <c>IsValidRowIndex(int)</c> method has the correct bounds.
        /// <para>
        /// This test covers 5 cases: The value below the lower boundary; the 
        /// lower boundary; a typical value within boundaries; the upper 
        /// boundary; and the value above the upper boundary.
        /// </para>
        /// </summary>
        [Test]
        public void TestIsValidRowIndex()
        {
            SquareGraph graph = new SquareGraph(10, 5);

            Assert.False(graph.IsValidRowIndex(-1));
            Assert.True(graph.IsValidRowIndex(0));
            Assert.True(graph.IsValidRowIndex(7));
            Assert.True(graph.IsValidRowIndex(9));
            Assert.False(graph.IsValidRowIndex(10));
        }

        /// <summary>
        /// Tests if the <c>IsValidColIndex(int)</c> method has the correct bounds.
        /// <para>
        /// This test covers 5 cases: The value below the lower boundary; the 
        /// lower boundary; a typical value within boundaries; the upper 
        /// boundary; and the value above the upper boundary.
        /// </para>
        /// </summary>
        [Test]
        public void TestIsValidColumnIndex()
        {
            SquareGraph graph = new SquareGraph(10, 5);

            Assert.False(graph.IsValidColumnIndex(-1));
            Assert.True(graph.IsValidColumnIndex(0));
            Assert.True(graph.IsValidColumnIndex(2));
            Assert.True(graph.IsValidColumnIndex(4));
            Assert.False(graph.IsValidColumnIndex(5));
        }

        /// <summary>
        /// Tests if <c>IsValid(Coordinate)</c> gives the proper results
        /// for various row values.
        /// <para>
        /// This test covers 5 cases: The value below the lower boundary; the 
        /// lower boundary; a typical value within boundaries; the upper 
        /// boundary; and the value above the upper boundary.
        /// </para>
        /// </summary>
        [Test]
        public void TestIsValidForRow()
        {
            SquareGraph graph = new SquareGraph(20, 10);
            Assert.False(graph.IsValid(new Coordinate(-1, 5)));
            Assert.True(graph.IsValid(new Coordinate(0, 5)));
            Assert.True(graph.IsValid(new Coordinate(12, 5)));
            Assert.True(graph.IsValid(new Coordinate(19, 5)));
            Assert.False(graph.IsValid(new Coordinate(20, 5)));
        }

        /// <summary>
        /// Tests if <c>IsValid(Coordinate)</c> gives the proper results
        /// for various column values.
        /// <para>
        /// This test covers 5 cases: The value below the lower boundary; the 
        /// lower boundary; a typical value within boundaries; the upper 
        /// boundary; and the value above the upper boundary.
        /// </para>
        /// </summary>
        [Test]
        public void TestIsValidForColumn()
        {
            SquareGraph graph = new SquareGraph(20, 10);
            Assert.False(graph.IsValid(new Coordinate(5, -1)));
            Assert.True(graph.IsValid(new Coordinate(5, 0)));
            Assert.True(graph.IsValid(new Coordinate(5, 4)));
            Assert.True(graph.IsValid(new Coordinate(5, 9)));
            Assert.False(graph.IsValid(new Coordinate(5, 10)));
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null coordinate is used in the <c>GetVertexAtCoords</c> method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetVertexAtCoordinateWithNullArgument()
        {
            SquareGraph graph = new SquareGraph(20, 20);
            graph.GetVertexAtCoordinate(null);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// an invalid coordinate is used in the <c>GetVertexAtCoords</c> method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetVertexAtCoordinateWithCoordinateOutOfRange()
        {
            SquareGraph graph = new SquareGraph(20, 20);
            graph.GetVertexAtCoordinate(new Coordinate(21, 21));
        }

        /// <summary>
        /// Tests if a correctly initialized Vertex is returned when a valid 
        /// Coordinate is used as a method argument.
        /// </summary>
        [Test]
        public void TestGetVertexAtCoordinateTypicalCase()
        {
            SquareGraph graph = new SquareGraph(20, 20);
            Coordinate coordinate = new Coordinate(9, 9);
            Vertex vertex = graph.GetVertexAtCoordinate(coordinate);
            
            Assert.AreEqual(coordinate, vertex.Coordinate);
            Assert.AreEqual(Property.EMPTY, vertex.Property);
        }

        /// <summary>
        /// Tests if manipulating the Vertex object returned by <c>GetVertexAtPosition</c>
        /// can be edited so that the state of the SquareGraph reflects the edit.
        /// </summary>
        [Test]
        public void TestGetVertexAtPositionAllowsEditing()
        {
            SquareGraph graph = new SquareGraph(4, 4);
            Assert.AreEqual(Property.EMPTY, graph.GetVertexAtPosition(2, 3).Property);

            graph.GetVertexAtPosition(2, 3).Property = Property.WALL;
            Assert.AreEqual(Property.WALL, graph.GetVertexAtPosition(2, 3).Property);
        }
    }
}
