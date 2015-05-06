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
			if (rows < 10 || cols < 10) {
				throw new ArgumentException("The amount of rows and the amount of cols" +
					"should both be at least 10.");
			}
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
		/// Returns the target coordinate.
		/// </summary>
		/// <returns>The target coordinate.</returns>
		public Coordinate GetTargetCoord() {
			return targetCoord;
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
			addRandomWalls();
		}
		/// <summary>
		/// Marks all the vertices on the same row from the start column
		/// to the end column as part of the critical path.
		/// </summary>
		/// <param name="row">The row index.</param>
		/// <param name="initcol">The initial column index.</param>
		/// <param name="endcol">The final column index.</param>
		private void pathFromToCol(int row, int initcol, int endcol) {
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
		/// <summary>
		/// Marks all the vertices on the same column from the start row
		/// to the end row as part of the critical path.
		/// </summary>
		/// <param name="initrow">The initial row index.</param>
		/// <param name="endrow">The final row index.</param>
		/// <param name="col">The column index.</param>
		private void pathFromToRow(int initrow, int endrow, int col) {
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
		/// <summary>
		/// Adds walls randomly to the map. The amount of added walls is less
		/// than half the amount of vertices in the map.
		/// </summary>
		private void addRandomWalls() {
			int max = (int) (sg.maxrow*sg.maxcol*4)/10;
			for(int i = 0; i < max; i++) {
				int randRow = randInt(0,sg.maxrow);
				int randCol = randInt(0,sg.maxcol);
				if(isNotYetOccupied(sg.GetVertexAtCoords(new Coordinate(randRow, randCol)))){
					sg.GetVertexAtCoords (new Coordinate (randRow, randCol)).prop = Property.WALL;
				}
			}
		}
		/// <summary>
		/// Checks if the vertex has the EMPTY property
		/// </summary>
		/// <returns><c>true</c>, if the vertex has the EMPTY property,
		/// <c>false</c> otherwise.</returns>
		/// <param name="v">The vertex.</param>
		private bool isNotYetOccupied(Vertex v) {
			return v.prop == Property.EMPTY;
		}
		/// <summary>
		/// Prints the randomly generated map. Useful for debugging.
		/// </summary>
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
		/// <summary>
		/// Finds a path, having northwest as the first quadrant.
		/// </summary>
		private void FindPathNorthWest() {
			int randRow = randInt(0,targetCoord.row);
			int randCol = randInt(0,targetCoord.col);
			int spare;
			pathFromToCol(targetCoord.row, targetCoord.col - 1, randCol);
			pathFromToRow(targetCoord.row, randRow, randCol);
			spare = randCol;
			randCol = randInt(targetCoord.col + 1, sg.maxcol);
			pathFromToCol(randRow, spare, randCol);
			spare = randRow;
			randRow = randInt(targetCoord.row+1, sg.maxrow);
			pathFromToRow(spare, randRow, randCol);
			sg.GetVertexAtCoords (new Coordinate (randRow, 0)).prop = Property.LASER;
			pathFromToCol(randRow, randCol, 1);
		}
		/// <summary>
		/// Finds a path, having northeast as the first quadrant.		
		/// </summary>
		private void FindPathNorthEast() {
			int randRow = randInt(0,targetCoord.row);
			int randCol = randInt(targetCoord.col + 1, sg.maxcol);
			int spare;
			pathFromToRow(targetCoord.row - 1, randRow, targetCoord.col);
			pathFromToCol(randRow, targetCoord.col, randCol);
			spare = randRow;
			randRow = randInt(targetCoord.row+1, sg.maxrow);
			pathFromToRow(spare,randRow,randCol);
			spare = randCol;
			randCol = randInt(0,targetCoord.col);
			pathFromToCol(randRow,spare,randCol);
			sg.GetVertexAtCoords(new Coordinate(0,randCol)).prop = Property.LASER;
			pathFromToRow(randRow,1,randCol);
		}
		/// <summary>
		/// Finds a path, having southeast as the first quadrant.
		/// </summary>
		private void FindPathSouthEast() {
			int randRow = randInt(targetCoord.row + 1, sg.maxrow);
			int randCol = randInt(targetCoord.col + 1, sg.maxcol);
			int spare;
			pathFromToCol(targetCoord.row, targetCoord.col + 1, randCol);
			pathFromToRow(targetCoord.row, randRow, randCol);
			spare = randCol;
			randCol = randInt(0,targetCoord.col);
			pathFromToCol(randRow,spare,randCol);
			spare = randRow;
			randRow = randInt(0,targetCoord.row);
			pathFromToRow(randRow,spare,randCol);
			sg.GetVertexAtCoords(new Coordinate(randRow,sg.maxcol-1)).prop = Property.LASER;
			pathFromToCol(randRow, randCol, sg.maxcol - 2);
		}
		/// <summary>
		/// Finds a path, having southwest as the first quadrant.
		/// </summary>
		private void FindPathSouthWest() {
			int randRow = randInt(targetCoord.row + 1, sg.maxrow);
			int randCol = randInt(0,targetCoord.col);
			int spare;
			pathFromToRow(targetCoord.row + 1, randRow, targetCoord.col);
			pathFromToCol(randRow, randCol, targetCoord.col);
			spare = randRow;
			randRow = randInt(0,targetCoord.row);
			pathFromToRow(randRow,spare,randCol);
			spare = randCol;
			randCol = randInt(targetCoord.col + 1, sg.maxcol);
			pathFromToCol(randRow, spare, randCol);
			sg.GetVertexAtCoords(new Coordinate(sg.maxrow-1,randCol)).prop = Property.LASER;
			pathFromToRow(randRow, sg.maxrow-2, randCol);
		}
	}
}
