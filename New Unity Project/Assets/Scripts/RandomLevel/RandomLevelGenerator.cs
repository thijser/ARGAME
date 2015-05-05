using System.Collections;
using System;
namespace RandomLevel{
	enum Quadrant {
		NORTHWEST,
		NORTHEAST,
		SOUTHEAST,
		SOUTHWEST
	}
	///<summary>
	///Generates randomized levels.
	///</summary>
	public class RandomLevelGenerator {
		private Coordinate targetCoord;
		Random r;
		public SquareGraph sg { get; set; }
		///<summary>
		///Creates a new RandomLevelGenerator with the given
		///size of the playing field.
		///</summary>
		///<param name="rows">The amount of rows</param>
		///<param name="cols">The amount of columns</param>
		public RandomLevelGenerator(int rows, int cols){
			sg = new SquareGraph (rows, cols);
			r = new Random (Environment.TickCount);
			run(sg);
		}
		public void run(SquareGraph sg) {
			//Specify row and column coordinate of target.
			int targetRowCoord = sg.maxrow/2;
			int targetColCoord = sg.maxcol/2;
			targetCoord = new Coordinate(targetRowCoord, targetColCoord);
			//Creating interim positions.
			sg.GetVertexAtCoords(targetCoord).IsTarget = true;
			int firstQuad = randInt(0,4);
			Quadrant q = DetermineQuad (firstQuad);
			findPath(sg, q);
			printGraph(sg);
		}
		/// <summary>
		/// Generates a random integer between min and max - 1.
		/// </summary>
		/// <returns>The int.</returns>
		/// <param name="min">The minimum value, inclusionary.</param>
		/// <param name="max">The maximum value, exclusionary.</param>
		private int randInt(int min, int max) {
			return r.Next (min, max);
		}
		Quadrant DetermineQuad(int i){
			if (i == 0)
				return Quadrant.NORTHWEST;
			else if (i == 1)
				return Quadrant.NORTHEAST;
			else if (i == 2)
				return Quadrant.SOUTHEAST;
			else
				return Quadrant.SOUTHWEST;
			
		}
		private void findPath(SquareGraph sg, Quadrant q) {
			if(q == Quadrant.NORTHWEST) {
				int randRow = randInt(0,targetCoord.row);
				int randCol = randInt(0,targetCoord.col);
				int spare;
				pathFromToCol(sg, targetCoord.row, targetCoord.col - 1, randCol);
				pathFromToRow(sg, targetCoord.row, randRow, randCol);
				spare = randCol;
				randCol = randInt(targetCoord.col + 1, sg.maxcol);
				pathFromToCol(sg, randRow, spare, randCol);
				spare = randRow;
				randRow = randInt(targetCoord.row+1, sg.maxrow);
				pathFromToRow(sg, spare, randRow, randCol);
				sg.GetVertexAtCoords(new Coordinate(randRow,0)).IsLaser = true;
				pathFromToCol(sg, randRow, randCol, 1);
			}
			else if (q == Quadrant.NORTHEAST){
				int randRow = randInt(0,targetCoord.row);
				int randCol = randInt(targetCoord.col + 1, sg.maxcol);
				int spare;
				pathFromToRow(sg, targetCoord.row - 1, randRow, targetCoord.col);
				pathFromToCol(sg, randRow, targetCoord.col, randCol);
				spare = randRow;
				randRow = randInt(targetCoord.row+1, sg.maxrow);
				pathFromToRow(sg,spare,randRow,randCol);
				spare = randCol;
				randCol = randInt(0,targetCoord.col);
				pathFromToCol(sg,randRow,spare,randCol);
				sg.GetVertexAtCoords(new Coordinate(0,randCol)).IsLaser = true;
				pathFromToRow(sg,randRow,1,randCol);
			}
			else if (q == Quadrant.SOUTHEAST) {
				int randRow = randInt(targetCoord.row + 1, sg.maxrow);
				int randCol = randInt(targetCoord.col + 1, sg.maxcol);
				int spare;
				pathFromToCol(sg, targetCoord.row, targetCoord.col + 1, randCol);
				pathFromToRow(sg, targetCoord.row, randRow, randCol);
				spare = randCol;
				randCol = randInt(0,targetCoord.col);
				pathFromToCol(sg,randRow,spare,randCol);
				spare = randRow;
				randRow = randInt(0,targetCoord.row);
				pathFromToRow(sg,randRow,spare,randCol);
				sg.GetVertexAtCoords(new Coordinate(randRow,sg.maxcol-1)).IsLaser = true;
				pathFromToCol(sg, randRow, randCol, sg.maxcol - 2);
			} else {
				int randRow = randInt(targetCoord.row + 1, sg.maxrow);
				int randCol = randInt(0,targetCoord.col);
				int spare;
				pathFromToRow(sg, targetCoord.row + 1, randRow, targetCoord.col);
				pathFromToCol(sg, randRow, randCol, targetCoord.col);
				spare = randRow;
				randRow = randInt(0,targetCoord.row);
				pathFromToRow(sg,randRow,spare,randCol);
				spare = randCol;
				randCol = randInt(targetCoord.col + 1, sg.maxcol);
				pathFromToCol(sg, randRow, spare, randCol);
				sg.GetVertexAtCoords(new Coordinate(sg.maxrow-1,randCol)).IsLaser = true;
				pathFromToRow(sg, randRow, sg.maxrow-2, randCol);
			}
			addRandomWalls(sg);
		}
		private void pathFromToCol(SquareGraph sg, int row, int initcol, int endcol) {
			if(initcol < endcol) {
				for(int i = initcol; i <= endcol; i++) {
					sg.GetVertexAtCoords(new Coordinate(row,i)).PartOfPath = true;
				}
			}
			else {
				for(int i = endcol; i <= initcol; i++) {
					sg.GetVertexAtCoords(new Coordinate(row,i)).PartOfPath = true;
				}
			}
		}
		private void pathFromToRow(SquareGraph sg, int initrow, int endrow, int col) {
			if(initrow < endrow) {
				for(int i = initrow; i <= endrow; i++) {
					sg.GetVertexAtCoords(new Coordinate(i,col)).PartOfPath = true;
				}
			} else {
				for(int i = endrow; i <= initrow; i++) {
					sg.GetVertexAtCoords(new Coordinate(i,col)).PartOfPath = true;
				}
			}
		}
		private void addRandomWalls(SquareGraph sg) {
			int max = (int) (sg.maxrow*sg.maxcol*4)/10;
			for(int i = 0; i < max; i++) {
				int randRow = randInt(0,sg.maxrow);
				int randCol = randInt(0,sg.maxcol);
				Console.Write (randRow);
				Console.Write (" ");
				Console.Write (randCol);
				Console.WriteLine ();
				if(isNotYetOccupied(sg.GetVertexAtCoords(new Coordinate(randRow, randCol)))){
					sg.GetVertexAtCoords(new Coordinate(randRow, randCol)).IsWall = true;
				}
			}
		}
		private bool isNotYetOccupied(Vertex v) {
			return !(v.IsLaser || v.PartOfPath || v.IsWall || v.IsTarget);
		}
		public void printGraph(SquareGraph sg) {
			for(int i = 0; i < sg.maxrow; i++) {
				for(int j = 0; j < sg.maxcol; j++) {
					Vertex v = sg.GetVertexAtCoords(new Coordinate(i,j));
					if(v.IsTarget){
						Console.Write("@");
					} else if (v.IsLaser) {
						Console.Write("L");
					} else if (v.PartOfPath) {
						Console.Write("#");
					} else if(v.IsWall) {
						Console.Write("!");
					} else {
						Console.Write(".");
					}
				}
				Console.Write("\n");
			}
		}
	}
}
