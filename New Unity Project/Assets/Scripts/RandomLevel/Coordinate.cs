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
        /// Gets the j coordinate.
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// Gets the i coordinate.
        /// </summary>
        public int Col { get; private set; }

        /// <summary>
        /// Adds a step in the given Direction.
        /// </summary>
        /// <param name="direction">The Direction towards which to step.</param>
        /// <returns>The translated Coordinate.</returns>
        public Coordinate StepTo(Direction direction)
        {
            return new Coordinate(
                this.Row + direction.GetVerticalComponent().GetAxisSign(),
                this.Col + direction.GetHorizontalComponent().GetAxisSign());
        }

        /// <summary>
        /// Tests whether this Coordinate is equal to the given object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True if this object is equal to <c>obj</c>, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            Coordinate that = obj as Coordinate;
            return this.Row == that.Row && this.Col == that.Col;
        }

        /// <summary>
        /// Returns a pseudo-unique Hash Code for this Coordinate.
        /// </summary>
        /// <returns>The Hash Code.</returns>
        public override int GetHashCode()
        {
            return (this.GetType().GetHashCode() * this.Row) ^ this.Col;
        }

        /// <summary>
        /// Returns a string representation of this Coordinate.
        /// </summary>
        /// <returns>A string representation of this Coordinate.</returns>
        public override string ToString()
        {
            return "(" + this.Row + ", " + this.Col + ")";
        }
    }
}
