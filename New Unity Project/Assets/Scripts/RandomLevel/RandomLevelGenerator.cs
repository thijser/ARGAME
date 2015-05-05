using UnityEngine;
using System.Collections;
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
		class Coordinate {
			private int row { get; set;}
			private int col { get; set;}
			public Coordinate(int xcoor, int ycoor){
				row = xcoor;
				col = ycoor;
			}
		}
		///<summary>
		///Creates a new RandomLevelGenerator with the given
		///size of the playing field.
		///</summary>
		///<param name="rows">The amount of rows</param>
		///<param name="cols">The amount of columns</param>
		public RandomLevelGenerator(int rows, int cols){
			SquareGraph sg = new SquareGraph (rows, cols);

		}
	}
}
