using UnityEngine;
using System.Collections;
namespace RandomLevel{

	/// <summary>
	/// Represents a grid-shaped graph with connections
	/// in the four cardinal directions.
	/// </summary>
	public class SquareGraph {
		private Vertex[,] squareMap;
		private int maxrow, maxcol;
		
		/// <summary>
		/// Creates a new SquareGraph instance of the specified size.
		/// </summary>
		/// <param name="rows">The amount of rows.</param>
		/// <param name="cols">The amount of columns.</param>
		public SquareGraph(int rows, int cols){
			maxrow = rows - 1;
			maxcol = cols - 1;
			squareMap = new Vertex[rows,cols];
			for(int i = 0; i < rows; i++){
				for(int j = 0; j < cols; j++){
					squareMap[i,j] = new Vertex();
				}
			}
			ConnectFull ();
		}
		private void ConnectFull(){
			for(int i = 0; i <= maxrow; i++){
				for(int j = 0; j <= maxcol; j++){
					Connect(i,j);
				}
			}
		}
		private void Connect(int i, int j){
			if (isValid (i, j - 1))
				squareMap [i, j].AddAdjacent (squareMap [i, j - 1]);
			if (isValid (i, j + 1))
				squareMap [i, j].AddAdjacent (squareMap [i, j + 1]);
			if (isValid (i - 1, j))
				squareMap [i, j].AddAdjacent (squareMap [i - 1, j]);
			if (isValid (i + 1, j))
				squareMap [i, j].AddAdjacent (squareMap [i + 1, j]);
		}
		private bool isValid(int row, int col){
			return !(row < 0 || row > maxrow || col < 0 || col > maxcol);
		}
	}
}
