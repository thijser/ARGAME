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

    /// <summary>
    /// Represents a grid-shaped graph with connections
    /// in the four cardinal directions.
    /// </summary>
    public class SquareGraph 
    {
        /// <summary>
        /// The square grid, representing the map.
        /// </summary>
        private Vertex[,] squareMap;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SquareGraph" /> class.
        /// </summary>
        /// <param name="rows">The amount of rows, a positive nonzero integer.</param>
        /// <param name="cols">The amount of columns, a positive nonzero integer.</param>
        public SquareGraph(int rows, int cols)
        {
            if (rows <= 0) 
            {
                throw new ArgumentException("The amount of rows should be more than 0.");
            }

            if (cols <= 0) 
            {
                throw new ArgumentException("The amount of cols should be more than 0.");
            }

            this.Maxrow = rows;
            this.Maxcol = cols;
            this.squareMap = new Vertex[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    this.squareMap[i, j] = new Vertex(new Coordinate(i, j));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the maximum amount of rows in the square graph.
        /// </summary>
        public int Maxrow { get; private set; }

        /// <summary>
        /// Gets a value indicating the maximum amount of columns in the square graph.
        /// </summary>
        public int Maxcol { get; private set; }

        /// <summary>
        /// Invokes the given Action for each Vertex on the graph.
        /// </summary>
        /// <param name="action">The Action to invoke, not null.</param>
        public void ForEach(Action<Vertex> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            foreach (var vertex in this.squareMap)
            {
                action.Invoke(vertex);
            }
        }

        /// <summary>
        /// Returns the vertex at the specified coordinate (a (Row, column)
        /// pair of integers). Throws an exception if the coordinate is out
        /// of bounds.
        /// </summary>
        /// <returns>The vertex at the given coordinate.</returns>
        /// <param name="coordinate">The coordinate of the vertex.</param>
        public Vertex GetVertexAtCoordinate(Coordinate coordinate)
        {
            if (!this.IsValid(coordinate))
            {
                throw new ArgumentException("Invalid Row-column pair.");
            }

            return this.squareMap[coordinate.Row, coordinate.Col];
        }

        /// <summary>
        /// Determines whether this coordinate is a valid location in
        /// the square graph.
        /// </summary>
        /// <returns><c>true</c> if the coordinate is within bounds,
        /// <c>false</c> otherwise.</returns>
        /// <param name="coordinate">The coordinate to be checked</param>
        public bool IsValid(Coordinate coordinate)
        {
            if (coordinate == null) 
            {
                throw new ArgumentNullException("coordinate");
            }

            return !(coordinate.Row < 0 || coordinate.Row >= this.Maxrow || coordinate.Col < 0 || coordinate.Col >= this.Maxcol);
        }
    }
}
