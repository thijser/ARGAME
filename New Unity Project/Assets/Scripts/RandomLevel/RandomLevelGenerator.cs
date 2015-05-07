//----------------------------------------------------------------------------
// <copyright file="RandomLevelGenerator.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace RandomLevel
{
    using System;

    /// <summary>
    /// A construct useful for describing the first quadrant.
    /// </summary>
    public enum Quadrant 
    {
        /// <summary>
        /// A value describing the northwest quadrant.
        /// </summary>
        NORTHWEST,

        /// <summary>
        /// A value describing the northeast quadrant.
        /// </summary>
        NORTHEAST,

        /// <summary>
        /// A value describing the southeast quadrant.
        /// </summary>
        SOUTHEAST,

        /// <summary>
        /// A value describing the southwest quadrant.
        /// </summary>
        SOUTHWEST
    }

    /// <summary>
    /// Generates randomized levels.
    /// </summary>
    public class RandomLevelGenerator 
    {
        /// <summary>
        /// A random number generator.
        /// </summary>
        private Random r;

        /// <summary>
        /// The square graph, representing the map.
        /// </summary>
        private SquareGraph sg;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomLevelGenerator"/> class.
        /// </summary>
        /// <param name="rows">The amount of rows</param>
        /// <param name="cols">The amount of columns</param>
        public RandomLevelGenerator(int rows, int cols)
        {
            if (rows < 10 || cols < 10) 
            {
                throw new ArgumentException("The amount of rows and the amount of cols" +
                    "should both be at least 10.");
            }

            this.sg = new SquareGraph(rows, cols);
            this.r = new Random(Environment.TickCount);
            this.Run();
        }

        /// <summary>
        /// Gets the coordinate of the target on the map.
        /// </summary>
        public Coordinate TargetCoord { get; private set; }

        /// <summary>
        /// Gets the first quadrant selected.
        /// </summary>
        public Quadrant Quad { get; private set; }

        /// <summary>
        /// Determines the quadrant from the given integer.
        /// </summary>
        /// <returns>The corresponding quadrant.</returns>
        /// <param name="integer">The integer.</param>
        public static Quadrant DetermineQuad(int integer)
        {
            switch (integer)
            {
                case 0: return Quadrant.NORTHWEST;
                case 1: return Quadrant.NORTHEAST;
                case 2: return Quadrant.SOUTHEAST;
                case 3: return Quadrant.SOUTHWEST;
                default: throw new ArgumentException("The parameter should be an integer between 0 and 3.");
            }
        }

        /// <summary>
        /// Returns the randomly generated map, in SquareGraph form.
        /// </summary>
        /// <returns>The randomly generated map.</returns>
        public SquareGraph ReturnRandomMap()
        {
            return this.sg;
        }

        /// <summary>
        /// Runs the map creation procedure. Called during construction
        /// of the object.
        /// </summary>
        private void Run()
        {
            ////Specify Row and column coordinate of target.
            int targetRowCoord = this.sg.Maxrow / 2;
            int targetColCoord = this.sg.Maxcol / 2;
            this.TargetCoord = new Coordinate(targetRowCoord, targetColCoord);
            this.sg.GetVertexAtCoordinate(this.TargetCoord).Prop = Property.TARGET;

            ////Determine first quadrant to plan a route to.
            int firstQuad = this.RandInt(0, 4);
            this.Quad = DetermineQuad(firstQuad);
            PathBuilder pathbuild = new PathBuilder(this.sg, this.TargetCoord);
            pathbuild.BuildPath(this.Quad);
        }

        /// <summary>
        /// Generates a random integer between min and max - 1.
        /// </summary>
        /// <returns>The random integer.</returns>
        /// <param name="min">The minimum value, inclusionary.</param>
        /// <param name="max">The maximum value, exclusionary.</param>
        private int RandInt(int min, int max)
        {
            return this.r.Next(min, max);
        }
    }
}
