using UnityEngine;
using System.Collections;
namespace RandomLevel{

	/// <summary>
	/// Generates randomized levels.
	/// </summary>
	public class RandomLevelGenerator {

		/// <summary>
		/// Creates a new RandomLevelGenerator with the given
		/// size of the playing field.
		/// </summary>
		/// <param name="rows">The amount of rows</param>
		/// <param name="cols">The amount of columns</param>
		public RandomLevelGenerator(int rows, int cols){
			SquareGraph sg = new SquareGraph (rows, cols);

		}
	}
}
