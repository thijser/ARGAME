//----------------------------------------------------------------------------
// <copyright file="Property.cs" company="Delft University of Technology">
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
	[TestFixture]
	/// <summary>
	/// Test class for the SquareGraph-class. All the options of all externally available
	/// methods are tested to maximize the amount of tested code.
	/// </summary>
	public class SquareGraphTesting
	{
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		/// <summary>
		/// Tests if the correct exception is thrown when
		/// the amount of rows is 0.
		/// </summary>
		public void SqGraphTestRowsIsZero() {
			new SquareGraph (0, 20);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		/// <summary>
		/// Tests if the correct exception is thrown when
		/// the amount of rows is negative.
		/// </summary>
		public void SqGraphTestRowsIsNegative() {
			new SquareGraph (-1, 20);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		/// <summary>
		/// Tests if the correct exception is thrown when
		/// the amount of cols is 0.
		/// </summary>
		public void SqGraphTestColsIsZero() {
			new SquareGraph (20, 0);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		/// <summary>
		/// Tests if the correct exception is thrown when
		/// the amount of cols is negative.
		/// </summary>
		public void SqGraphTestColsIsNegative() {
			new SquareGraph (20, -1);
		}
		[Test]
		/// <summary>
		/// Tests some SquareGraph behaviour if the given parameters 
		/// are valid.
		/// </summary>
		public void SqGraphTestValid() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.AreNotEqual (sg, null);
			Assert.AreEqual (sg.Maxrow, 20);
			Assert.AreEqual (sg.Maxcol, 20);
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		/// <summary>
		/// Tests if the correct exception is thrown when a null
		/// coordinate is passed as a parameter in the IsValid-method.
		/// </summary>
		public void SqGraphTestCoordIsNull() {
			SquareGraph sg = new SquareGraph (20, 20);
			sg.IsValid (null);
		}
		[Test]
		/// <summary>
		/// Tests for invalidity of several invalid coordinates
		/// (these coordinates have been designed to test every single
		/// possibility of invalidity).
		/// </summary>
		public void SqGraphTestCoordIsInValid() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.False(sg.IsValid(new Coordinate(20,9)));
			Assert.False(sg.IsValid(new Coordinate(-1,9)));
			Assert.False(sg.IsValid(new Coordinate(4,26)));
			Assert.False(sg.IsValid(new Coordinate(4,-1)));
		}
		[Test]
		/// <summary>
		/// Tests for validity of a valid coordinate.
		/// </summary>
		public void SqGraphTestCoordIsValid() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.True(sg.IsValid(new Coordinate(12,9)));
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		/// <summary>
		/// Tests if the correct exception is thrown when
		/// a null coordinate is used in the GetVertexAtCoords method.
		/// </summary>
		public void SqGraphTestCoordIsNullv2() {
			SquareGraph sg = new SquareGraph (20, 20);
			sg.GetVertexAtCoordinate (null);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		/// <summary>
		/// Tests if the correct exception is thrown when
		/// an invalid coordinate is used in the GetVertexAtCoords method.
		/// </summary>
		public void SqGraphTestInvalidCoord() {
			SquareGraph sg = new SquareGraph (20, 20);
			sg.GetVertexAtCoordinate (new Coordinate(21,21));
		}
		[Test]
		/// <summary>
		/// Tests if a non-null vertex is returned when a valid coordinate
		/// is used as a method argument.
		/// </summary>
		public void SqGraphTestValidCoord() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.NotNull(sg.GetVertexAtCoordinate (new Coordinate(9,9)));
		}
	}
}

