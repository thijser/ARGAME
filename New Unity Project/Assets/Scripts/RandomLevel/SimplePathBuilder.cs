﻿//----------------------------------------------------------------------------
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
    using UnityEngine;

    /// <summary>
    /// Builds paths that go in a straight line.
    /// </summary>
    public class SimplePathBuilder
    {
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

            this.Graph = graph;
        }

        /// <summary>
        /// Gets the SquareGraph this PathBuilder builds a path in.
        /// </summary>
        public SquareGraph Graph { get; private set; }
        
        /// <summary>
        /// Marks all the vertices on the same Row from the start column
        /// to the end column as part of the critical path.
        /// <para>
        /// The provided indices are not included in the Path.
        /// </para>
        /// </summary>
        /// <param name="row">The Row index.</param>
        /// <param name="initialColumn">The initial column index.</param>
        /// <param name="endColumn">The final column index.</param>
        public void PathFromToCol(int row, int initialColumn, int endColumn)
        {
            this.VerifyRowIndex(row, "row", "The row must be between 0 and " + this.Graph.Maxrow);
            this.VerifyColumnIndex(initialColumn, "initialColumn", "The initial column must be between 0 and " + this.Graph.Maxcol);
            this.VerifyColumnIndex(endColumn, "endColumn", "The end column must be between 0 and " + this.Graph.Maxcol);

            int start = Math.Min(initialColumn, endColumn);
            int end = Math.Max(initialColumn, endColumn);
            for (int i = start + 1; i < end; i++)
            {
                this.Graph.GetVertexAtPosition(row, i).Property = Property.PARTOFPATH;
            }
        }
        
        /// <summary>
        /// Marks all the vertices on the same column from the start Row
        /// to the end Row as part of the critical path.
        /// <para>
        /// The provided indices are not included in the Path.
        /// </para>
        /// </summary>
        /// <param name="initialRow">The initial row index.</param>
        /// <param name="endRow">The final row index.</param>
        /// <param name="column">The column index.</param>
        public void PathFromToRow(int initialRow, int endRow, int column)
        {
            this.VerifyRowIndex(initialRow, "initialRow", "The initial row must be between 0 and " + this.Graph.Maxrow);
            this.VerifyRowIndex(endRow, "endRow", "The end row must be between 0 and " + this.Graph.Maxrow);
            this.VerifyColumnIndex(column, "column", "The column must be between 0 and " + this.Graph.Maxcol);
            
            int start = Math.Min(initialRow, endRow);
            int end = Math.Max(initialRow, endRow);
            for (int i = start + 1; i < end; i++)
            {
                this.Graph.GetVertexAtPosition(i, column).Property = Property.PARTOFPATH;
            }
        }

        /// <summary>
        /// Constructs a path in the graph between the specified coordinates, starting 
        /// in the specified direction.
        /// <para>
        /// The start and end Coordinates themselves are not included in the Path.
        /// </para>
        /// </summary>
        /// <param name="from">The starting Coordinate, not null.</param>
        /// <param name="to">The end Coordinate, not null.</param>
        /// <param name="initialDirection">The initial Direction, must be valid and non-combined.</param>
        public void ConstructPath(Coordinate from, Coordinate to, Direction initialDirection)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            
            if (!initialDirection.IsValid() || initialDirection.IsCombinedDirection())
            {
                throw new ArgumentException("Valid, non-combined Direction required", "initialDirection");
            }

            int startColumn = from.Col;
            int startRow = from.Row;
            int endColumn = to.Col;
            int endRow = to.Row;
            
            if (initialDirection.GetHorizontalComponent() == Direction.None)
            {
                // We first need to go vertically.
                this.PathFromToRow(startRow, endRow, startColumn);
                this.PathFromToCol(endRow, startColumn, endColumn);
            }
            else
            {
                // We first need to go horizontally.
                this.PathFromToCol(startRow, startColumn, endColumn);
                this.PathFromToRow(startRow, endRow, endColumn);
            }
        }

        /// <summary>
        /// Tests whether the given Coordinate is free.
        /// </summary>
        /// <param name="coordinate">The Coordinate to test</param>
        /// <returns>True if the Coordinate is free, false otherwise.</returns>
        public bool IsFree(Coordinate coordinate)
        {
            return this.Graph.GetVertexAtCoordinate(coordinate).Property == Property.EMPTY;
        }

        /// <summary>
        /// Tests whether all provided Coordinates are free.
        /// </summary>
        /// <param name="coordinates">The Coordinates to test.</param>
        /// <returns>True if all Coordinates are free, false otherwise.</returns>
        /// <seealso cref="SimplePathBuilder.IsFree"/>
        public bool AllFree(params Coordinate[] coordinates)
        {
            return Array.TrueForAll(coordinates, this.IsFree);
        }

        /// <summary>
        /// Verifies if the given row index is valid, throws an 
        /// <c>ArgumentOutOfRangeException</c> otherwise.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="message">The exception message.</param>
        private void VerifyRowIndex(int row, string argumentName, string message)
        {
            if (!this.Graph.IsValidRowIndex(row))
            {
                throw new ArgumentOutOfRangeException(argumentName, row, message);
            }
        }

        /// <summary>
        /// Verifies if the given column index is valid, throws an 
        /// <c>ArgumentOutOfRangeException</c> otherwise.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="message">The exception message.</param>
        private void VerifyColumnIndex(int column, string argumentName, string message)
        {
            if (!this.Graph.IsValidColumnIndex(column))
            {
                throw new ArgumentOutOfRangeException(argumentName, column, message);
            }
        }
    }
}
