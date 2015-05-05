﻿using System.Collections;
using System;
namespace RandomLevel{
	
	/// <summary>
	/// Represents a grid-shaped graph with connections
	/// in the four cardinal directions.
	/// </summary>
	public class SquareGraph {
		private Vertex[,] squareMap;
		public int maxrow { get; set;}
		public int maxcol { get; set;}
		
		/// <summary>
		/// Creates a new SquareGraph instance of the specified size.
		/// </summary>
		/// <param name="rows">The amount of rows.</param>
		/// <param name="cols">The amount of columns.</param>
		public SquareGraph(int rows, int cols){
			maxrow = rows;
			maxcol = cols;
			squareMap = new Vertex[rows,cols];
			for(int i = 0; i < rows; i++){
				for(int j = 0; j < cols; j++){
					squareMap[i,j] = new Vertex();
				}
			}
		}
		/// <summary>
		/// Returns the vertex at the specified coordinate (a (row, column)
		/// pair of integers). Throws an exception if the coordinate is out
		/// of bounds.
		/// </summary>
		/// <returns>The vertex at coords.</returns>
		/// <param name="coord">The coordinate of the vertex.</param>
		internal Vertex GetVertexAtCoords(Coordinate coord)
		{
			if (!IsValid (coord.row, coord.col)) {
				throw new ArgumentException ();
			}
			return squareMap [coord.row, coord.col];
		}
		/// <summary>
		/// Determines whether this coordinate is a valid location in
		/// the square graph.
		/// </summary>
		/// <returns><c>true</c> if the coordinate is within bounds,
		/// <c>false</c> otherwise.
		/// <param name="c">The coordinate to be checked</param>
		bool IsValid(Coordinate c){
			return !(c.row < 0 || c.row >= maxrow || c.col < 0 || c.col >= maxcol);
		}
	}
}
