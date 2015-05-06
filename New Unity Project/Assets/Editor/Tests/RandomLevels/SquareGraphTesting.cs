//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace AssemblyCSharpEditor.Tests.RandomLevels
{
	using System;
	using RandomLevel;
	using NUnit.Framework;
	[TestFixture]
	public class SquareGraphTesting
	{
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SqGraphTestRowsIsZero() {
			new SquareGraph (0, 20);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SqGraphTestRowsIsNegative() {
			new SquareGraph (-1, 20);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SqGraphTestColsIsZero() {
			new SquareGraph (20, 0);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SqGraphTestColsIsNegative() {
			new SquareGraph (20, -1);
		}
		[Test]
		public void SqGraphTestValid() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.AreNotEqual (sg, null);
			Assert.AreEqual (sg.maxrow, 20);
			Assert.AreEqual (sg.maxcol, 20);
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SqGraphTestCoordIsNull() {
			SquareGraph sg = new SquareGraph (20, 20);
			sg.IsValid (null);
		}
		[Test]
		public void SqGraphTestCoordIsInValid() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.False(sg.IsValid(new Coordinate(20,9)));
			Assert.False(sg.IsValid(new Coordinate(-1,9)));
			Assert.False(sg.IsValid(new Coordinate(4,26)));
			Assert.False(sg.IsValid(new Coordinate(4,-1)));
		}
		[Test]
		public void SqGraphTestCoordIsValid() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.True(sg.IsValid(new Coordinate(12,9)));
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SqGraphTestCoordIsNullv2() {
			SquareGraph sg = new SquareGraph (20, 20);
			sg.GetVertexAtCoords (null);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SqGraphTestInvalidCoord() {
			SquareGraph sg = new SquareGraph (20, 20);
			sg.GetVertexAtCoords (new Coordinate(21,21));
		}
		[Test]
		public void SqGraphTestValidCoord() {
			SquareGraph sg = new SquareGraph (20, 20);
			Assert.NotNull(sg.GetVertexAtCoords (new Coordinate(9,9)));
		}
	}
}

