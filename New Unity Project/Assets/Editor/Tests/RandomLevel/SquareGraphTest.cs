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
    /// Test class for the SquareGraph-class. All the options of all externally available
    /// methods are tested to maximize the amount of tested code.
    /// </summary>
    [TestFixture]
    public class SquareGraphTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the correct exception is thrown when
        /// the amount of rows is 0.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SqGraphTestRowsIsZero()
        {
            new SquareGraph(0, 20);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// the amount of rows is negative.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SqGraphTestRowsIsNegative()
        {
            new SquareGraph(-1, 20);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// the amount of cols is 0.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SqGraphTestColsIsZero()
        {
            new SquareGraph(20, 0);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// the amount of cols is negative.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SqGraphTestColsIsNegative()
        {
            new SquareGraph(20, -1);
        }

        /// <summary>
        /// Tests some SquareGraph behavior if the given parameters 
        /// are valid.
        /// </summary>
        [Test]
        public void SqGraphTestValid()
        {
            SquareGraph sg = new SquareGraph(20, 20);
            Assert.AreNotEqual(sg, null);
            Assert.AreEqual(sg.Maxrow, 20);
            Assert.AreEqual(sg.Maxcol, 20);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when a null
        /// coordinate is passed as a parameter in the IsValid-method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqGraphTestCoordIsNull()
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
        public void TestValidRowIndex()
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
        public void TestValidColumnIndex()
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
        /// for various cases.
        /// </summary>
        [Test]
        public void TestIsValidForRow()
        {
            SquareGraph sg = new SquareGraph(20, 10);
            Assert.False(sg.IsValid(new Coordinate(-1, 5)));
            Assert.True(sg.IsValid(new Coordinate(0, 5)));
            Assert.True(sg.IsValid(new Coordinate(12, 5)));
            Assert.True(sg.IsValid(new Coordinate(19, 5)));
            Assert.False(sg.IsValid(new Coordinate(20, 5)));
        }

        /// <summary>
        /// Tests for validity of a valid coordinate.
        /// </summary>
        [Test]
        public void SqGraphTestCoordIsValid()
        {
            SquareGraph sg = new SquareGraph(20, 20);
            Assert.True(sg.IsValid(new Coordinate(12, 9)));
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null coordinate is used in the <c>GetVertexAtCoords</c> method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqGraphTestCoordIsNullv2()
        {
            SquareGraph sg = new SquareGraph(20, 20);
            sg.GetVertexAtCoordinate(null);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// an invalid coordinate is used in the <c>GetVertexAtCoords</c> method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SqGraphTestInvalidCoord()
        {
            SquareGraph sg = new SquareGraph(20, 20);
            sg.GetVertexAtCoordinate(new Coordinate(21, 21));
        }

        /// <summary>
        /// Tests if a non-null vertex is returned when a valid coordinate
        /// is used as a method argument.
        /// </summary>
        [Test]
        public void SqGraphTestValidCoord()
        {
            SquareGraph sg = new SquareGraph(20, 20);
            Assert.NotNull(sg.GetVertexAtCoordinate(new Coordinate(9, 9)));
        }
    }
}
