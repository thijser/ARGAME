//----------------------------------------------------------------------------
// <copyright file="WallBuilder.cs" company="Delft University of Technology">
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
    /// Builds randomized walls in a level.
    /// </summary>
    public class WallBuilder
    {
        /// <summary>
        /// The Random instance used to generate randomized maps.
        /// </summary>
        private Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="WallBuilder"/> class.
        /// </summary>
        public WallBuilder()
        {
            this.random = new Random(Environment.TickCount);
        }

        /// <summary>
        /// Adds walls randomly to the map. The amount of added walls is less
        /// than half the amount of vertices in the map.
        /// </summary>
        /// <param name = "graph">The SquareGraph object.</param>
        public void AddRandomWalls(SquareGraph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            int max = graph.Maxrow * graph.Maxcol * 4 / 10;
            for (int i = 0; i < max; i++)
            {
                int randRow = this.random.Next(0, graph.Maxrow);
                int randCol = this.random.Next(0, graph.Maxcol);
                Coordinate coordinate = new Coordinate(randRow, randCol);
                if (graph.GetVertexAtCoordinate(coordinate).Prop == Property.EMPTY)
                {
                    graph.GetVertexAtCoordinate(coordinate).Prop = Property.WALL;
                }
            }
        }
    }
}
