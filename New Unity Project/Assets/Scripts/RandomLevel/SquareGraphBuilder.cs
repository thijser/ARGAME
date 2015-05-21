//----------------------------------------------------------------------------
// <copyright file="SquareGraph.cs" company="Delft University of Technology">
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
    using System.Collections;
    using System.Text;

    /// <summary>
    /// Initializes the square graph by initializing
    /// all vertices in the 2D matrix.
    /// </summary>
    public class SquareGraphBuilder
    {
        /// <summary>
        /// The amount of rows in this 2D matrix.
        /// </summary>
        private int rows;

        /// <summary>
        /// The amount of columns in this 2D matrix.
        /// </summary>
        private int cols;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareGraphBuilder" /> class.
        /// </summary>
        /// <param name="rows">The amount of rows in the 2D matrix.</param>
        /// <param name="cols">The amount of columns in the 2D matrix.</param>
        public SquareGraphBuilder(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
        }

        /// <summary>
        /// Initializes the 2D matrix and returns it.
        /// </summary>
        /// <returns></returns>
        public Vertex[,] Init()
        {
            Vertex[,] graph = new Vertex[rows, cols];
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    graph[i, j] = new Vertex(new Coordinate(i, j));
                }
            }

            return graph;
        }
    }
}
