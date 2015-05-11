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

	[TestFixture]
	public class PathBuilderTest
	{
		/// <summary>
		/// Tests if the correct exception is thrown when a null reference
		/// is used in the constructor as SquareGraph.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullSquareGraphTest() {
			new PathBuilder(null, new Coordinate (0, 0));
		}

		/// <summary>
		/// Tests if the correct exception is thrown when a null reference
		/// is used in the constructor as Coordinate.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullCoordinateTest() {
			new PathBuilder(new SquareGraph(1,1), null);
		}

		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is NW.
		/// </summary>
		[Test]
		public void PathBuilderNWFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.NORTHWEST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (10, 9)).Prop == Property.PARTOFPATH);
		}

		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is NE.
		/// </summary>
		[Test]
		public void PathBuilderNEFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.NORTHEAST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (9, 10)).Prop == Property.PARTOFPATH);
		}

		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is SE.
		/// </summary>
		[Test]
		public void PathBuilderSEFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.SOUTHEAST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (10, 11)).Prop == Property.PARTOFPATH);
		}

		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is SW.
		/// </summary>
		[Test]
		public void PathBuilderSWFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.SOUTHWEST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (11, 10)).Prop == Property.PARTOFPATH);
		}
	}
}

