//----------------------------------------------------------------------------
// <copyright file="SquareGraphStringBuilder.cs" company="Delft University of Technology">
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
    /// This is a utility class that takes a 2D
    /// matrix of vertices and makes a string representation.
    /// </summary>
    public class SquareGraphStringBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SquareGraphStringBuilder" /> class.
        /// </summary>
        private SquareGraphStringBuilder()
        {

        }

        /// <summary>
        /// Returns a string representation of this SquareMap.
        /// </summary>
        /// <param name="maxrow">The max. amount of rows.</param>
        /// <param name="maxcol">The max. amount of columns.</param>
        /// <param name="graph">The square graph.</param>
        /// <returns></returns>
        public static string ToString(int maxrow, int maxcol, Vertex[,] graph)
        {
            // The final String length is the amount of vertices (rows * columns) plus the
            // amount of characters in a single newline (max. 2) times the amount of rows.
            StringBuilder builder = new StringBuilder((maxcol + 2) * maxrow);
            for (int column = 0; column < maxcol; column++)
            {
                for (int row = 0; row < maxrow; row++)
                {
                    builder.Append(GetVertexSymbol(graph[row, column]));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns a representative character for the given vertex.
        /// </summary>
        /// <param name="v">The row.</param>
        /// <returns>The character describing the Vertex.</returns>
        private static char GetVertexSymbol(Vertex v)
        {
            Property property = v.Property;
            switch (property)
            {
                case Property.EMPTY: return ' ';
                case Property.LASER: return 'O';
                case Property.PARTOFPATH: return '.';
                case Property.TARGET: return 'X';
                case Property.WALL: return '#';
                default: throw new ArgumentException("Unknown Property at given position: " + property);
            }
        }
    }
}
