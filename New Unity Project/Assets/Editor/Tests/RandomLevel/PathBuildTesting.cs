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
	public class PathBuildTesting
	{
		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		/// <summary>
		/// Tests if the correct exception is thrown when a null reference
		/// is used in the constructor as SquareGraph.
		/// </summary>
		public void PathBuilderConstructionNullSGTest() {
			new PathBuilder(null, new Coordinate (0, 0));
		}

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		/// <summary>
		/// Tests if the correct exception is thrown when a null reference
		/// is used in the constructor as Coordinate.
		/// </summary>
		public void PathBuilderConstructionNullCoordTest() {
			new PathBuilder(new SquareGraph(1,1), null);
		}
		[Test]
		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is NW.
		/// </summary>
		public void PathBuilderNWFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.NORTHWEST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (10, 9)).Prop == Property.PARTOFPATH);
		}
		[Test]
		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is NE.
		/// </summary>
		public void PathBuilderNEFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.NORTHEAST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (9, 10)).Prop == Property.PARTOFPATH);
		}
		[Test]
		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is SE.
		/// </summary>
		public void PathBuilderSEFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.SOUTHEAST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (10, 11)).Prop == Property.PARTOFPATH);
		}
		[Test]
		/// <summary>
		/// Tests if the first vertex labeled as part of the critical
		/// path is positioned correctly, given the first quadrant is SW.
		/// </summary>
		public void PathBuilderSWFirstTest(){
			SquareGraph square = new SquareGraph (20, 20);
			PathBuilder pb = new PathBuilder (square, new Coordinate (10, 10));
			pb.BuildPath (Quadrant.SOUTHWEST);
			Assert.IsTrue (square.GetVertexAtCoordinate (new Coordinate (11, 10)).Prop == Property.PARTOFPATH);
		}
	}
}

