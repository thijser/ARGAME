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
	/// <summary>
	/// Unit tests for the WallBuilder class.
	/// </summary>
	public class WallBuildTesting
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		/// <summary>
		/// Tests if the correct exception is thrown
		/// when a null object is passed to the AddRandomWalls method.
		/// </summary>
		public void WallBuildTestSGNull() 
		{
			WallBuilder wallbuild = new WallBuilder ();
			wallbuild.AddRandomWalls (null);
		}
		[Test]
		/// <summary>
		/// Checks if at least one vertex has been labeled as a wall.
		/// </summary>
		public void WallBuildTestAtLeastOneWall() 
		{
			WallBuilder wb = new WallBuilder ();
			SquareGraph sg = new SquareGraph (20, 20);
			wb.AddRandomWalls (sg);
			bool check = false;
			for (int i = 0; i < 20; i++) 
			{
				for(int j = 0; j < 20; j++) 
				{
					if(sg.GetVertexAtCoords(new Coordinate(i,j)).prop == Property.WALL) 
					{
						check = true;
					}
				}
			}
			Assert.IsTrue (check);
		} 
		[Test]
		/// <summary>
		/// Checks if at least one vertex has been labeled as empty space.
		/// </summary>
		public void WallBuildTestAtLeastOneEmptySpace() 
		{
			WallBuilder wb = new WallBuilder ();
			SquareGraph sg = new SquareGraph (20, 20);
			wb.AddRandomWalls (sg);
			bool check = false;
			for (int i = 0; i < 20; i++) 
			{
				for(int j = 0; j < 20; j++) 
				{
					if(sg.GetVertexAtCoords(new Coordinate(i,j)).prop == Property.EMPTY) 
					{
						check = true;
					}
				}
			}
			Assert.IsTrue (check);
		}
	}
}

