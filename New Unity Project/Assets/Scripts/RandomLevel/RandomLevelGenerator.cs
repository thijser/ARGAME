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
		private Random r;
		private SquareGraph sg;
		///<summary>
		///Creates a new RandomLevelGenerator with the given
		///size of the playing field.
		///</summary>
		///<param name="rows">The amount of rows</param>
		///<param name="cols">The amount of columns</param>
		public RandomLevelGenerator(int rows, int cols){
			sg = new SquareGraph (rows, cols);
			r = new Random (Environment.TickCount);
			run();
		}
		/// <summary>
		/// Runs the map creation procedure. Called during construction
		/// of the object.
		/// </summary>
		public void run() {
			//Specify row and column coordinate of target.
			int targetRowCoord = sg.maxrow/2;
			int targetColCoord = sg.maxcol/2;
			targetCoord = new Coordinate(targetRowCoord, targetColCoord);
			sg.GetVertexAtCoords(targetCoord).prop = Property.TARGET;
			//Determine first quadrant to plan a route to.
			int firstQuad = randInt(0,4);
			Quadrant q = DetermineQuad (firstQuad);
			findPath(q);
			printGraph();
		}
		/// <summary>
		/// Returns the randomly generated map, in SquareGraph form.
		/// </summary>
		/// <returns>The randomly generated map.</returns>
		public SquareGraph ReturnRandomMap(){
			return sg;
		}
		/// <summary>
		/// Generates a random integer between min and max - 1.
		/// </summary>
		/// <returns>The random integer.</returns>
		/// <param name="min">The minimum value, inclusionary.</param>
		/// <param name="max">The maximum value, exclusionary.</param>
		private int randInt(int min, int max) {
			return r.Next (min, max);
		}
		/// <summary>
		/// Determines the quadrant from the given integer.
		/// </summary>
		/// <returns>The corresponding quadrant.</returns>
		/// <param name="i">The integer.</param>
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
		/// <summary>
		/// Constructs a path from target to laser.
		/// </summary>
		/// <param name="q"> The first quadrant.</param>
		private void findPath(Quadrant q) {
			if(q == Quadrant.NORTHWEST) {
				FindPathNorthWest ();
			}
			else if (q == Quadrant.NORTHEAST){
				FindPathNorthEast ();
			}
			else if (q == Quadrant.SOUTHEAST) {
				FindPathSouthEast ();
			} 
			else {
				FindPathSouthWest ();
			}
			addRandomWalls(sg);
		}
		private void pathFromToCol(SquareGraph sg, int row, int initcol, int endcol) {
			if(initcol < endcol) {
				for(int i = initcol; i <= endcol; i++) {
					sg.GetVertexAtCoords (new Coordinate (row, i)).prop = Property.PARTOFPATH;
				}
			}
			else {
				for(int i = endcol; i <= initcol; i++) {
					sg.GetVertexAtCoords(new Coordinate(row,i)).prop = Property.PARTOFPATH;
				}
			}
		}
		private void pathFromToRow(SquareGraph sg, int initrow, int endrow, int col) {
			if(initrow < endrow) {
				for(int i = initrow; i <= endrow; i++) {
					sg.GetVertexAtCoords(new Coordinate(i,col)).prop = Property.PARTOFPATH;
				}
			} else {
				for(int i = endrow; i <= initrow; i++) {
					sg.GetVertexAtCoords(new Coordinate(i,col)).prop = Property.PARTOFPATH;
				}
			}
		}
		private void addRandomWalls(SquareGraph sg) {
			int max = (int) (sg.maxrow*sg.maxcol*4)/10;
			for(int i = 0; i < max; i++) {
				int randRow = randInt(0,sg.maxrow);
				int randCol = randInt(0,sg.maxcol);
				if(isNotYetOccupied(sg.GetVertexAtCoords(new Coordinate(randRow, randCol)))){
					sg.GetVertexAtCoords (new Coordinate (randRow, randCol)).prop = Property.WALL;
				}
			}
		}
		private bool isNotYetOccupied(Vertex v) {
			return v.prop == Property.EMPTY;
		}
		public void printGraph() {
			for(int i = 0; i < sg.maxrow; i++) {
				for(int j = 0; j < sg.maxcol; j++) {
					Vertex v = sg.GetVertexAtCoords(new Coordinate(i,j));
					if(v.prop == Property.TARGET){
						Console.Write("@");
					} else if (v.prop == Property.LASER) {
						Console.Write("L");
					} else if (v.prop == Property.PARTOFPATH) {
						Console.Write("#");
					} else if(v.prop == Property.WALL) {
						Console.Write("!");
					} else {
						Console.Write(".");
					}
				}
				Console.Write("\n");
			}
		}
		private void FindPathNorthWest() {
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
			sg.GetVertexAtCoords (new Coordinate (randRow, 0)).prop = Property.LASER;
			pathFromToCol(sg, randRow, randCol, 1);
		}
		private void FindPathNorthEast() {
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
			sg.GetVertexAtCoords(new Coordinate(0,randCol)).prop = Property.LASER;
			pathFromToRow(sg,randRow,1,randCol);
		}
		private void FindPathSouthEast() {
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
			sg.GetVertexAtCoords(new Coordinate(randRow,sg.maxcol-1)).prop = Property.LASER;
			pathFromToCol(sg, randRow, randCol, sg.maxcol - 2);
		}
		private void FindPathSouthWest() {
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
			sg.GetVertexAtCoords(new Coordinate(sg.maxrow-1,randCol)).prop = Property.LASER;
			pathFromToRow(sg, randRow, sg.maxrow-2, randCol);
		}
	}
}
