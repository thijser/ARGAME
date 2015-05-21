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
    /// Generates randomized levels.
    /// </summary>
    public class RandomLevelGenerator 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomLevelGenerator"/> class.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="cols">The amount of columns.</param>
        public RandomLevelGenerator(int rows, int cols)
        {
            if (rows < 10 || cols < 10) 
            {
                throw new ArgumentException("The amount of rows and the amount of cols" +
                    "should both be at least 10.");
            }

            this.Graph = new SquareGraph(rows, cols);
        }

        /// <summary>
        /// Gets the coordinate of the target on the map.
        /// </summary>
        public Coordinate TargetCoordinate { get; private set; }

        /// <summary>
        /// Gets the first quadrant selected.
        /// </summary>
        public Direction EmitterDirection { get; private set; }

        /// <summary>
        /// Gets the randomly generated map.
        /// </summary>
        public SquareGraph Graph { get; private set; }

        /// <summary>
        /// Runs the map creation procedure.
        /// </summary>
        public void Run()
        {
            // Specify Row and i coordinate of target.
            int targetRowCoord = this.Graph.Maxrow / 2;
            int targetColCoord = this.Graph.Maxcol / 2;
            this.TargetCoordinate = new Coordinate(targetRowCoord, targetColCoord);
            this.BuildPath(4);
            this.CreateWalls();
        }

        /// <summary>
        /// Constructs a path from target to laser.
        /// </summary>
        /// <param name="iterations">The amount of iterations of the path builder.</param>
        /// <exception cref="ArgumentException">If <c>iterations</c> is not positive.</exception>
        public void BuildPath(int iterations)
        {
            if (iterations <= 0)
            {
                throw new ArgumentException("Number of iterations should be positive", "iterations");
            }

            PathBuilder builder = new PathBuilder(this.Graph, this.TargetCoordinate);
            Coordinate target = this.TargetCoordinate;
            for (int step = iterations; step > 0; step--)
            {
                target = builder.FindPathSegment(target);
            }
            
            // The variable 'target' now contains the location of the Laser beam.
            this.EmitterDirection = this.FindLaserBeam(target, Direction.East, Direction.West, Direction.North, Direction.South);
            this.Graph.GetVertexAtCoordinate(target).Property = Property.LASER;

            // The Property 'TargetCoordinate' contains the LaserTarget position.
            this.Graph.GetVertexAtCoordinate(this.TargetCoordinate).Property = Property.TARGET;
        }

        /// <summary>
        /// Adds randomized walls to the map.
        /// </summary>
        public void CreateWalls()
        {
            WallBuilder builder = new WallBuilder();
            builder.AddRandomWalls(this.Graph);
        }
        
        /// <summary>
        /// Finds the Direction in which a Laser Beam is placed.
        /// </summary>
        /// <param name="coordinate">The Coordinate</param>
        /// <param name="directions">The Directions to consider.</param>
        /// <returns>The Direction, or <c>Direction.None</c> if it was not found.</returns>
        private Direction FindLaserBeam(Coordinate coordinate, params Direction[] directions)
        {
            return Array.Find(directions, c => this.Graph.GetVertexAtCoordinate(coordinate.StepTo(c)).Property == Property.PARTOFPATH);
        }
    }
}
