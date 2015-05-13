namespace RandomLevel
{
    using System;

    /// <summary>
    /// Builds paths that go in a straight line.
    /// </summary>
    class SimplePathBuilder
    {
        /// <summary>
        /// The SquareGraph this PathBuilder builds a path in.
        /// </summary>
        private SquareGraph graph;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathBuilder"/> class.
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
        /// <param name="initcol">The initial column index.</param>
        /// <param name="endcol">The final column index.</param>
        public void PathFromToCol(int row, int initcol, int endcol)
        {
            if (!(IsValidRowIndex(row) && IsValidColIndex(initcol) && IsValidColIndex(endcol)))
            {
                throw new ArgumentException("At least one of the indices supplied is invalid, given the square graph used.");
            }

            int start = Math.Min(initcol, endcol);
            int end = Math.Max(initcol, endcol);
            for (int i = start; i <= end; i++)
            {
                this.graph.GetVertexAtCoordinate(new Coordinate(row, i)).Property = Property.PARTOFPATH;
            }
        }
        
        /// <summary>
        /// Marks all the vertices on the same column from the start Row
        /// to the end Row as part of the critical path.
        /// </summary>
        /// <param name="initrow">The initial Row index.</param>
        /// <param name="endrow">The final Row index.</param>
        /// <param name="col">The column index.</param>
        public void PathFromToRow(int initrow, int endrow, int col)
        {
            if(!(IsValidRowIndex(initrow) && IsValidRowIndex(endrow) && IsValidColIndex(col)))
            {
                throw new ArgumentException("At least one of the indices supplied is invalid, given the square graph used.");
            }

            int start = Math.Min(initrow, endrow);
            int end = Math.Max(initrow, endrow);
            for (int i = start; i <= end; i++)
            {
                this.graph.GetVertexAtCoordinate(new Coordinate(i, col)).Property = Property.PARTOFPATH;
            }
        }

        /// <summary>
        /// Checks if supplied parameter is a valid row index.
        /// </summary>
        /// <param name="rowind">The row index supplied.</param>
        /// <returns>True if the row index supplied is within bounds, false otherwise.</returns>
        public bool IsValidRowIndex(int rowind)
        {
            return !(rowind < 0 || rowind >= graph.Maxrow);
        }

        /// <summary>
        /// Checks is supplied parameter is a valid column index.
        /// </summary>
        /// <param name="colind">The column index supplied.</param>
        /// <returns>True if the column index supplied is within bounds, false otherwise.</returns>
        public bool IsValidColIndex(int colind)
        {
            return !(colind < 0 || colind >= graph.Maxcol);
        }
    }
}
