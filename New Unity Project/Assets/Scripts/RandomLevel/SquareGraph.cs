using UnityEngine;
using System.Collections;
namespace RandomLevel{

	///<summary>
	///Represents a grid-shaped graph with connections
	///in the four cardinal directions.
	///</summary>
	public class SquareGraph {
		private Vertex[,] squareMap;
		public int maxrow { get; set;}
		public int maxcol { get; set;}
		
		///<summary>
		///Creates a new SquareGraph instance of the specified size.
		///</summary>
		///<param name="rows">The amount of rows.</param>
		///<param name="cols">The amount of columns.</param>
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
		Vertex getVertexAtCoords(Coordinate coord)
		{
			if (!IsValid (coord.row, coord.col)) {
				throw new UnityException();
			}
			int r = coord.row;
			int c = coord.col;
			return squareMap [r, c];
		}
		bool IsValid(int row, int col){
			return !(row < 0 || row >= maxrow || col < 0 || col >= maxcol);
		}
	}
}
