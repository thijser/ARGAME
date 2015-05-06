using System.Collections;
using System;
namespace RandomLevel{
	public enum Quadrant {
		NORTHWEST,
		NORTHEAST,
		SOUTHEAST,
		SOUTHWEST
	}
	///<summary>
	///Generates randomized levels.
	///</summary>
	public class RandomLevelGenerator {
		internal Coordinate targetCoord { get; set;}
		private Random r;
		private SquareGraph sg;
		public Quadrant q { get; set; }
		///<summary>
		///Creates a new RandomLevelGenerator with the given
		///size of the playing field.
		///</summary>
		///<param name="rows">The amount of rows</param>
		///<param name="cols">The amount of columns</param>
		public RandomLevelGenerator(int rows, int cols)
		{
			if (rows < 10 || cols < 10) 
			{
				throw new ArgumentException("The amount of rows and the amount of cols" +
					"should both be at least 10.");
			}
			sg = new SquareGraph (rows, cols);
			r = new Random (Environment.TickCount);
			Run();
		}
		/// <summary>
		/// Runs the map creation procedure. Called during construction
		/// of the object.
		/// </summary>
		public void Run() 
		{
			//Specify row and column coordinate of target.
			int targetRowCoord = sg.maxrow/2;
			int targetColCoord = sg.maxcol/2;
			targetCoord = new Coordinate(targetRowCoord, targetColCoord);
			sg.GetVertexAtCoords(targetCoord).prop = Property.TARGET;
			//Determine first quadrant to plan a route to.
			int firstQuad = RandInt(0,4);
			q = DetermineQuad (firstQuad);
			PathBuilder pathbuild = new PathBuilder (sg, targetCoord);
			pathbuild.BuildPath(q);
		}
		/// <summary>
		/// Returns the randomly generated map, in SquareGraph form.
		/// </summary>
		/// <returns>The randomly generated map.</returns>
		public SquareGraph ReturnRandomMap()
		{
			return sg;
		}
		/// <summary>
		/// Generates a random integer between min and max - 1.
		/// </summary>
		/// <returns>The random integer.</returns>
		/// <param name="min">The minimum value, inclusionary.</param>
		/// <param name="max">The maximum value, exclusionary.</param>
		private int RandInt(int min, int max) 
		{
			return r.Next (min, max);
		}
		/// <summary>
		/// Determines the quadrant from the given integer.
		/// </summary>
		/// <returns>The corresponding quadrant.</returns>
		/// <param name="i">The integer.</param>
		static Quadrant DetermineQuad(int i)
		{
			switch (i) 
			{
				case 0: return Quadrant.NORTHWEST;
				case 1: return Quadrant.NORTHEAST;
				case 2: return Quadrant.SOUTHEAST;
				case 3: return Quadrant.SOUTHWEST;
				default: throw new ArgumentException("The parameter should be an integer between 0 and 3.");
			}
		}
	}
}
