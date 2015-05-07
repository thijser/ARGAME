//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace RandomLevel
{
	using System;
	using NUnit.Framework;
	[TestFixture]
	/// <summary>
	/// Unit tests for the RandomLevelGenerator class.
	/// </summary>
	public class RandomLevelGenTest
	{
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		/// <summary>
		/// Tests if the correct exception is thrown once the constructor
		/// is called with an amount of rows less than ten.
		/// </summary>
		public void CreateRLGRowsLessThanTen() {
			new RandomLevelGenerator (9, 10);
		}
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		/// <summary>
		/// Tests if the correct exception is thrown once the constructor
		/// is called with an amount of columns less than ten.
		/// </summary>
		public void CreateRLGColsLessThanTen() {
			new RandomLevelGenerator (10, 9);
		}
		[Test]
		/// <summary>
		/// Tests if the created random map is not null if the constructor
		/// is called with valid parameters.
		/// </summary>
		public void CreateRLGResultNonZeroTest() {
			RandomLevelGenerator randomlevs = new RandomLevelGenerator (10, 10);
			Assert.NotNull (randomlevs.ReturnRandomMap ());
		}
		[Test]
		/// <summary>
		/// Tests the determineQuadrant-method completely.
		/// </summary>
		public void DetermineQuadrantTest() {
			Assert.IsTrue (RandomLevelGenerator.DetermineQuad (0) == Quadrant.NORTHWEST);
			Assert.IsTrue (RandomLevelGenerator.DetermineQuad (1) == Quadrant.NORTHEAST);
			Assert.IsTrue (RandomLevelGenerator.DetermineQuad (2) == Quadrant.SOUTHEAST);
			Assert.IsTrue (RandomLevelGenerator.DetermineQuad (3) == Quadrant.SOUTHWEST);
		}
	}
}

