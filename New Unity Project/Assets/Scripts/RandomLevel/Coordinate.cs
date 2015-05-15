//----------------------------------------------------------------------------
// <copyright file="Coordinate.cs" company="Delft University of Technology">
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
    /// A coordinate class, to specify a coordinate in the map
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomLevel.Coordinate"/> class.
        /// </summary>
        /// <param name="row"> The row coordinate.</param>
        /// <param name="column"> The column coordinate.</param>
        public Coordinate(int row, int column)
        {
            this.Row = row;
            this.Col = column;
        }

        /// <summary>
        /// Gets the row coordinate.
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// Gets the column coordinate.
        /// </summary>
        public int Col { get; private set; }
    }
}
