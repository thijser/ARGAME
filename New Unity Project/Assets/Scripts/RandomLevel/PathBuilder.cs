//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
namespace RandomLevel
{
	public class PathBuilder
	{
		private Coordinate targetCoord;
		private SquareGraph sg;
		private Random random;
		public PathBuilder (SquareGraph graph, Coordinate target)
		{
			if (graph == null) 
			{
				throw new ArgumentNullException("The square graph provided should not be null.");
			}
			if (target == null) 
			{
				throw new ArgumentNullException("The target coordinate provided should not be null.");
			}
			sg = graph;
			targetCoord = target;
			random = new Random (Environment.TickCount);
		}

		/// <summary>
		/// Constructs a path from target to laser.
		/// </summary>
		/// <param name="q"> The first quadrant.</param>
		public void BuildPath(Quadrant q) 
		{
			if(q == Quadrant.NORTHWEST) 
			{
				FindPathNorthWest ();
			}
			else if (q == Quadrant.NORTHEAST)
			{
				FindPathNorthEast ();
			}
			else if (q == Quadrant.SOUTHEAST) 
			{
				FindPathSouthEast ();
			} 
			else 
			{
				FindPathSouthWest ();
			}
			WallBuilder wallbuild = new WallBuilder ();
			wallbuild.AddRandomWalls(sg);
		}

		/// <summary>
		/// Finds a path, having northwest as the first quadrant.
		/// </summary>
		private void FindPathNorthWest() 
		{
			int randRow = RandInt(0,targetCoord.row);
			int randCol = RandInt(0,targetCoord.col);
			int spare;
			PathFromToCol(targetCoord.row, targetCoord.col - 1, randCol);
			PathFromToRow(targetCoord.row, randRow, randCol);
			spare = randCol;
			randCol = RandInt(targetCoord.col + 1, sg.maxcol);
			PathFromToCol(randRow, spare, randCol);
			spare = randRow;
			randRow = RandInt(targetCoord.row+1, sg.maxrow);
			PathFromToRow(spare, randRow, randCol);
			sg.GetVertexAtCoords (new Coordinate (randRow, 0)).prop = Property.LASER;
			PathFromToCol(randRow, randCol, 1);
		}

		/// <summary>
		/// Finds a path, having northeast as the first quadrant.		
		/// </summary>
		private void FindPathNorthEast() 
		{
			int randRow = RandInt(0,targetCoord.row);
			int randCol = RandInt(targetCoord.col + 1, sg.maxcol);
			int spare;
			PathFromToRow(targetCoord.row - 1, randRow, targetCoord.col);
			PathFromToCol(randRow, targetCoord.col, randCol);
			spare = randRow;
			randRow = RandInt(targetCoord.row+1, sg.maxrow);
			PathFromToRow(spare,randRow,randCol);
			spare = randCol;
			randCol = RandInt(0,targetCoord.col);
			PathFromToCol(randRow,spare,randCol);
			sg.GetVertexAtCoords(new Coordinate(0,randCol)).prop = Property.LASER;
			PathFromToRow(randRow,1,randCol);
		}

		/// <summary>
		/// Finds a path, having southeast as the first quadrant.
		/// </summary>
		private void FindPathSouthEast() 
		{
			int randRow = RandInt(targetCoord.row + 1, sg.maxrow);
			int randCol = RandInt(targetCoord.col + 1, sg.maxcol);
			int spare;
			PathFromToCol(targetCoord.row, targetCoord.col + 1, randCol);
			PathFromToRow(targetCoord.row, randRow, randCol);
			spare = randCol;
			randCol = RandInt(0,targetCoord.col);
			PathFromToCol(randRow,spare,randCol);
			spare = randRow;
			randRow = RandInt(0,targetCoord.row);
			PathFromToRow(randRow,spare,randCol);
			sg.GetVertexAtCoords(new Coordinate(randRow,sg.maxcol-1)).prop = Property.LASER;
			PathFromToCol(randRow, randCol, sg.maxcol - 2);
		}

		/// <summary>
		/// Finds a path, having southwest as the first quadrant.
		/// </summary>
		private void FindPathSouthWest() 
		{
			int randRow = RandInt(targetCoord.row + 1, sg.maxrow);
			int randCol = RandInt(0,targetCoord.col);
			int spare;
			PathFromToRow(targetCoord.row + 1, randRow, targetCoord.col);
			PathFromToCol(randRow, randCol, targetCoord.col);
			spare = randRow;
			randRow = RandInt(0,targetCoord.row);
			PathFromToRow(randRow,spare,randCol);
			spare = randCol;
			randCol = RandInt(targetCoord.col + 1, sg.maxcol);
			PathFromToCol(randRow, spare, randCol);
			sg.GetVertexAtCoords(new Coordinate(sg.maxrow-1,randCol)).prop = Property.LASER;
			PathFromToRow(randRow, sg.maxrow-2, randCol);
		}

		/// <summary>
		/// Generates a random integer between min and max - 1.
		/// </summary>
		/// <returns>The random integer.</returns>
		/// <param name="min">The minimum value, inclusionary.</param>
		/// <param name="max">The maximum value, exclusionary.</param>
		private int RandInt(int min, int max) 
		{
			return random.Next (min, max);
		}
		/// <summary>
		/// Marks all the vertices on the same row from the start column
		/// to the end column as part of the critical path.
		/// </summary>
		/// <param name="row">The row index.</param>
		/// <param name="initcol">The initial column index.</param>
		/// <param name="endcol">The final column index.</param>
		private void PathFromToCol(int row, int initcol, int endcol) {
			int start = Math.Min (initcol, endcol);
			int end = Math.Max (initcol, endcol);
			for(int i = start; i <= end; i++) 
			{
				sg.GetVertexAtCoords (new Coordinate (row, i)).prop = Property.PARTOFPATH;
			}
		}

		/// <summary>
		/// Marks all the vertices on the same column from the start row
		/// to the end row as part of the critical path.
		/// </summary>
		/// <param name="initrow">The initial row index.</param>
		/// <param name="endrow">The final row index.</param>
		/// <param name="col">The column index.</param>
		private void PathFromToRow(int initrow, int endrow, int col) 
		{
			int start = Math.Min (initrow, endrow);
			int end = Math.Max (initrow, endrow);
			for(int i = start; i <= end; i++) 
			{
				sg.GetVertexAtCoords(new Coordinate(i,col)).prop = Property.PARTOFPATH;
			}
		}
	}
}

