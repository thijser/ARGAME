//----------------------------------------------------------------------------
// <copyright file="PathBuilder.cs" company="Delft University of Technology">
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
    using Debug = UnityEngine.Debug;
    
    /// <summary>
    /// Builds a path in a graph leading to a predefined target.
    /// </summary>
    public class PathBuilder : SimplePathBuilder
    {
        /// <summary>
        /// The Random instance used for generating random numbers.
        /// </summary>
        private Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathBuilder"/> class.
        /// </summary>
        /// <param name="graph">The SquareGraph.</param>
        /// <param name="target">The target Coordinate.</param>
        public PathBuilder(SquareGraph graph, Coordinate target) : base(graph)
        {
            if (graph == null) 
            {
                throw new ArgumentNullException("graph");
            }

            if (target == null) 
            {
                throw new ArgumentNullException("target");
            }

            this.TargetCoordinate = target;
            this.random = new Random(Environment.TickCount);
        }
        
        /// <summary>
        /// Gets the target Coordinate.
        /// </summary>
        public Coordinate TargetCoordinate { get; private set; }

        /// <summary>
        /// Returns a random, valid Direction for building a path.
        /// </summary>
        /// <returns>The random Direction.</returns>
        public Direction RandomDirection()
        {
            Direction dir = Direction.None;
            bool north = this.random.NextDouble() < 0.5;
            bool west = this.random.NextDouble() < 0.5;
            dir |= north ? Direction.North : Direction.South;
            dir |= west ? Direction.West : Direction.East;
            return dir;
        }

        /// <summary>
        /// Returns the horizontal distance in the given direction from the given
        /// column to the edge of the graph.
        /// </summary>
        /// <param name="direction">The direction, must be either <c>East</c> or <c>West</c>.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The distance to the edge of the graph.</returns>
        public int GetHorizontalLength(Direction direction, int column)
        {
            int distance = direction.GetHorizontalComponent().Opposite().GetAxisSign() * column;
            if (distance < 0)
            {
                distance += this.Graph.Maxcol;
            }

            Debug.Log("Horizontal Length: " + column + " (" + direction + ") => " + distance);
            return distance;
        }

        /// <summary>
        /// Returns the vertical distance in the given direction from the given
        /// row to the edge of the graph.
        /// </summary>
        /// <param name="direction">The direction, must be either <c>North</c> or <c>South</c>.</param>
        /// <param name="row">The row index.</param>
        /// <returns>The distance to the edge of the graph.</returns>
        public int GetVerticalLength(Direction direction, int row)
        {
            int distance = direction.GetVerticalComponent().Opposite().GetAxisSign() * row;
            if (distance < 0)
            {
                distance += this.Graph.Maxrow;
            }

            Debug.Log("Vertical Length: " + row + " (" + direction + ") => " + distance);
            return distance;
        }

        /// <summary>
        /// Finds a random path segment that connects to the given target.
        /// </summary>
        /// <param name="target">The target on the path.</param>
        /// <returns>The next target on the path, can be equal to <c>target</c>.</returns>
        public Coordinate FindPathSegment(Coordinate target) 
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            // We first get a random Direction and its horizontal and vertical components.
            Direction direction = this.RandomDirection();
            Direction horizontal = direction.GetHorizontalComponent();
            Direction vertical = direction.GetVerticalComponent();

            // Then, we find a random point relative to the target in the range that is 
            // still within the bounds of the SquareGraph.
            int horizontalOffset = horizontal.GetAxisSign() * 
                this.RandInt(0, this.GetHorizontalLength(horizontal, target.Col));
            int verticalOffset = vertical.GetAxisSign() * 
                this.RandInt(0, this.GetVerticalLength(vertical, target.Row));
            Debug.Log("Trying relative point: " + new Coordinate(verticalOffset, horizontalOffset) + " from " + target);
            Coordinate nextTarget = new Coordinate(
                target.Row + verticalOffset,
                target.Col + horizontalOffset);
            Debug.Log("Trying next target: " + nextTarget);
            Coordinate cornerA = new Coordinate(target.Row, nextTarget.Col);
            Coordinate cornerB = new Coordinate(nextTarget.Row, target.Col);

            // Now we check if the Path is free.
            // A Path is possible if both positions are free of Laser beams, and the corner of the 
            // path segment is free or Laser beams (because there could be mirrors on all of these 
            // positions, and they would interfere with those existing laser beams).
            if (this.AllFree(target, nextTarget, cornerA))
            {
                this.ConstructPath(target, nextTarget, horizontal);
                return nextTarget;
            }

            if (this.AllFree(target, nextTarget, cornerB))
            {
                this.ConstructPath(target, nextTarget, vertical);
                return nextTarget;
            }
            
            // We failed to find a valid next target.
            // We return the original target without modifications so that the next step can take 
            // a different potential next target.
            Debug.Log("Failed to find next target from " + target + "(tried Coordinate " + nextTarget + ")");
            return target;
        }

        /// <summary>
        /// Generates a random integer between a and b.
        /// </summary>
        /// <returns>The random integer.</returns>
        /// <param name="a">The minimum value, inclusionary.</param>
        /// <param name="b">The maximum value, exclusionary.</param>
        private int RandInt(int a, int b) 
        {
            return this.random.Next(Math.Min(a, b), Math.Max(a, b));
        }
    }
}
