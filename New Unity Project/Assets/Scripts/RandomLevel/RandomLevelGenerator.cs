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
		///<summary>
		///Creates a new RandomLevelGenerator with the given
		///size of the playing field.
		///</summary>
		///<param name="rows">The amount of rows</param>
		///<param name="cols">The amount of columns</param>
		public RandomLevelGenerator(int rows, int cols){
			SquareGraph sg = new SquareGraph (rows, cols);
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
			// nextInt is normally exclusive of the top value,
			// so add 1 to make it inclusive
			// (deleted here, as it would cause problems).
			int randomNum = Random.Range (min, max);
			
			return randomNum;
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
	}
}
