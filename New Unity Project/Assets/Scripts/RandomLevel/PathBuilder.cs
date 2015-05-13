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
    
    /// <summary>
    /// Builds a path in a graph leading to a predefined target.
    /// </summary>
    public class PathBuilder
    {
        /// <summary>
        /// The SquareGraph this PathBuilder builds a path in.
        /// </summary>
        private SquareGraph graph;

        /// <summary>
        /// The Random instance used for generating random numbers.
        /// </summary>
        private Random random;

        /// <summary>
        /// The SimplePathBuilder instance that builds straight paths.
        /// </summary>
        private SimplePathBuilder spb;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathBuilder"/> class.
        /// </summary>
        /// <param name="graph">The SquareGraph.</param>
        /// <param name="target">The target Coordinate.</param>
        public PathBuilder(SquareGraph graph, Coordinate target)
        {
            if (graph == null) 
            {
                throw new ArgumentNullException("graph");
            }

            if (target == null) 
            {
                throw new ArgumentNullException("target");
            }

            this.spb = new SimplePathBuilder(graph);
            this.graph = graph;
            this.TargetCoordinate = target;
            this.random = new Random(Environment.TickCount);
        }
        
        /// <summary>
        /// Gets the target Coordinate.
        /// </summary>
        public Coordinate TargetCoordinate { get; private set; }

        /// <summary>
        /// Constructs a path from target to laser.
        /// </summary>
        /// <param name="quad"> The first quadrant.</param>
        public void BuildPath(Quadrant quad) 
        {
            switch (quad) 
            {
                case Quadrant.NORTHWEST: 
                    this.FindPathNorthWest();
                    break;
                case Quadrant.NORTHEAST: 
                    this.FindPathNorthEast();
                    break;
                case Quadrant.SOUTHEAST: 
                    this.FindPathSouthEast();
                    break;
                case Quadrant.SOUTHWEST: 
                    this.FindPathSouthWest();
                    break;
                default: 
                    throw new ArgumentException("The specified quadrant was not valid.");
            }

            WallBuilder wallbuild = new WallBuilder();
            wallbuild.AddRandomWalls(this.graph);
        }

        /// <summary>
        /// Finds a path, having northwest as the first quadrant.
        /// </summary>
        private void FindPathNorthWest() 
        {
            int randRow = this.RandInt(0, this.TargetCoordinate.Row);
            int randCol = this.RandInt(0, this.TargetCoordinate.Col);
            int spare;
            this.spb.PathFromToCol(this.TargetCoordinate.Row, this.TargetCoordinate.Col - 1, randCol);
            this.spb.PathFromToRow(this.TargetCoordinate.Row, randRow, randCol);
            spare = randCol;
            randCol = this.RandInt(this.TargetCoordinate.Col + 1, this.graph.Maxcol);
            this.spb.PathFromToCol(randRow, spare, randCol);
            spare = randRow;
            randRow = this.RandInt(this.TargetCoordinate.Row + 1, this.graph.Maxrow);
            this.spb.PathFromToRow(spare, randRow, randCol);
            this.graph.GetVertexAtCoordinate(new Coordinate(randRow, 0)).Property = Property.LASER;
            this.spb.PathFromToCol(randRow, randCol, 1);
        }

        /// <summary>
        /// Finds a path, having northeast as the first quadrant.
        /// </summary>
        private void FindPathNorthEast() 
        {
            int randRow = this.RandInt(0, this.TargetCoordinate.Row);
            int randCol = this.RandInt(this.TargetCoordinate.Col + 1, this.graph.Maxcol);
            int spare;
            this.spb.PathFromToRow(this.TargetCoordinate.Row - 1, randRow, this.TargetCoordinate.Col);
            this.spb.PathFromToCol(randRow, this.TargetCoordinate.Col, randCol);
            spare = randRow;
            randRow = this.RandInt(this.TargetCoordinate.Row + 1, this.graph.Maxrow);
            this.spb.PathFromToRow(spare, randRow, randCol);
            spare = randCol;
            randCol = this.RandInt(0, this.TargetCoordinate.Col);
            this.spb.PathFromToCol(randRow, spare, randCol);
            this.graph.GetVertexAtCoordinate(new Coordinate(0, randCol)).Property = Property.LASER;
            this.spb.PathFromToRow(randRow, 1, randCol);
        }

        /// <summary>
        /// Finds a path, having southeast as the first quadrant.
        /// </summary>
        private void FindPathSouthEast() 
        {
            int randRow = this.RandInt(this.TargetCoordinate.Row + 1, this.graph.Maxrow);
            int randCol = this.RandInt(this.TargetCoordinate.Col + 1, this.graph.Maxcol);
            int spare;
            this.spb.PathFromToCol(this.TargetCoordinate.Row, this.TargetCoordinate.Col + 1, randCol);
            this.spb.PathFromToRow(this.TargetCoordinate.Row, randRow, randCol);
            spare = randCol;
            randCol = this.RandInt(0, this.TargetCoordinate.Col);
            this.spb.PathFromToCol(randRow, spare, randCol);
            spare = randRow;
            randRow = this.RandInt(0, this.TargetCoordinate.Row);
            this.spb.PathFromToRow(randRow, spare, randCol);
            this.graph.GetVertexAtCoordinate(new Coordinate(randRow, this.graph.Maxcol - 1)).Property = Property.LASER;
            this.spb.PathFromToCol(randRow, randCol, this.graph.Maxcol - 2);
        }

        /// <summary>
        /// Finds a path, having southwest as the first quadrant.
        /// </summary>
        private void FindPathSouthWest() 
        {
            int randRow = this.RandInt(this.TargetCoordinate.Row + 1, this.graph.Maxrow);
            int randCol = this.RandInt(0, this.TargetCoordinate.Col);
            int spare;
            this.spb.PathFromToRow(this.TargetCoordinate.Row + 1, randRow, this.TargetCoordinate.Col);
            this.spb.PathFromToCol(randRow, randCol, this.TargetCoordinate.Col);
            spare = randRow;
            randRow = this.RandInt(0, this.TargetCoordinate.Row);
            this.spb.PathFromToRow(randRow, spare, randCol);
            spare = randCol;
            randCol = this.RandInt(this.TargetCoordinate.Col + 1, this.graph.Maxcol);
            this.spb.PathFromToCol(randRow, spare, randCol);
            this.graph.GetVertexAtCoordinate(new Coordinate(this.graph.Maxrow - 1, randCol)).Property = Property.LASER;
            this.spb.PathFromToRow(randRow, this.graph.Maxrow - 2, randCol);
        }

        /// <summary>
        /// Generates a random integer between min and max - 1.
        /// </summary>
        /// <returns>The random integer.</returns>
        /// <param name="min">The minimum value, inclusionary.</param>
        /// <param name="max">The maximum value, exclusionary.</param>
        private int RandInt(int min, int max) 
        {
            return this.random.Next(min, max);
        }
    }
}
