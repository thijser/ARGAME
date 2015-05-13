//----------------------------------------------------------------------------
// <copyright file="SimplePathBuilder.cs" company="Delft University of Technology">
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
    /// Builds paths that go in a straight line.
    /// </summary>
    public class SimplePathBuilder
    {
        /// <summary>
        /// The SquareGraph this PathBuilder builds a path in.
        /// </summary>
        private SquareGraph graph;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePathBuilder"/> class.
        /// </summary>
        /// <param name="graph">The SquareGraph.</param>
        public SimplePathBuilder(SquareGraph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            this.graph = graph;
        }
        
        /// <summary>
        /// Marks all the vertices on the same Row from the start column
        /// to the end column as part of the critical path.
        /// </summary>
        /// <param name="row">The Row index.</param>
        /// <param name="initialColumn">The initial column index.</param>
        /// <param name="endColumn">The final column index.</param>
        public void PathFromToCol(int row, int initialColumn, int endColumn)
        {
            if (!(this.IsValidRowIndex(row) && this.IsValidColIndex(initialColumn) && this.IsValidColIndex(endColumn)))
            {
                throw new ArgumentException("At least one of the indices supplied is invalid, given the square graph used.");
            }

            int start = Math.Min(initialColumn, endColumn);
            int end = Math.Max(initialColumn, endColumn);
            for (int i = start; i <= end; i++)
            {
                this.graph.GetVertexAtCoordinate(new Coordinate(row, i)).Property = Property.PARTOFPATH;
            }
        }
        
        /// <summary>
        /// Marks all the vertices on the same column from the start Row
        /// to the end Row as part of the critical path.
        /// </summary>
        /// <param name="initialRow">The initial Row index.</param>
        /// <param name="endRow">The final Row index.</param>
        /// <param name="column">The column index.</param>
        public void PathFromToRow(int initialRow, int endRow, int column)
        {
            if (!(this.IsValidRowIndex(initialRow) && this.IsValidRowIndex(endRow) && this.IsValidColIndex(column)))
            {
                throw new ArgumentException("At least one of the indices supplied is invalid, given the square graph used.");
            }

            int start = Math.Min(initialRow, endRow);
            int end = Math.Max(initialRow, endRow);
            for (int i = start; i <= end; i++)
            {
                this.graph.GetVertexAtCoordinate(new Coordinate(i, column)).Property = Property.PARTOFPATH;
            }
        }

        /// <summary>
        /// Checks if supplied parameter is a valid row index.
        /// </summary>
        /// <param name="rowIndex">The row index supplied.</param>
        /// <returns>True if the row index supplied is within bounds, false otherwise.</returns>
        public bool IsValidRowIndex(int rowIndex)
        {
            return !(rowIndex < 0 || rowIndex >= this.graph.Maxrow);
        }

        /// <summary>
        /// Checks is supplied parameter is a valid column index.
        /// </summary>
        /// <param name="columnIndex">The column index supplied.</param>
        /// <returns>True if the column index supplied is within bounds, false otherwise.</returns>
        public bool IsValidColIndex(int columnIndex)
        {
            return !(columnIndex < 0 || columnIndex >= this.graph.Maxcol);
        }
    }
}
