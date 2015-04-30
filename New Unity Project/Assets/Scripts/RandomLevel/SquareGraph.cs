using UnityEngine;
using System.Collections;
namespace RandomLevel{
	class SquareGraph {
		private Vertex[,] squareMap;
		int maxrow, maxcol;
		public SquareGraph(int rows, int cols){
			maxrow = rows - 1;
			maxcol = cols - 1;
			squareMap = new Vertex[rows][cols];
			for(int i = 0; i < rows; i++){
				for(int j = 0; j < cols; j++){
					squareMap[i,j] = new Vertex();
				}
			}
		}
		private void ConnectFull(){

		}
		private void ConnectCorners(){
			squareMap [0,0].AddAdjacent (squareMap [0,1]);
			squareMap [0,0].AddAdjacent (squareMap [1,0]);
			squareMap [0,maxcol].AddAdjacent (squareMap [0,maxcol - 1]);
			squareMap [0,maxcol].AddAdjacent (squareMap [1,maxcol]);
			squareMap [maxrow,0].AddAdjacent (squareMap [maxrow,1]);
			squareMap [maxrow,0].AddAdjacent (squareMap [maxrow - 1,0]);
		}
	}
}