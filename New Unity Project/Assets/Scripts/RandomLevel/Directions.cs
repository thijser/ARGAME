//----------------------------------------------------------------------------
// <copyright file="Directions.cs" company="Delft University of Technology">
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
    /// An enumeration for identifying directions.
    /// </summary>
    [Flags]
    public enum Direction
    {
        /// <summary>
        /// A value describing an unknown quadrant.
        /// </summary>
        None = 0,

        /// <summary>
        /// A value describing the North Direction.
        /// </summary>
        North = 1,

        /// <summary>
        /// A value describing the West Direction.
        /// </summary>
        West = 2,

        /// <summary>
        /// A value describing the South Direction.
        /// </summary>
        South = 4,

        /// <summary>
        /// A value describing the East Direction.
        /// </summary>
        East = 8,

        /// <summary>
        /// A value describing the North-West Direction.
        /// </summary>
        NorthWest = North | West,

        /// <summary>
        /// A value describing the North-East Direction.
        /// </summary>
        NorthEast = North | East,

        /// <summary>
        /// A value describing the South-West Direction.
        /// </summary>
        SouthWest = South | West,

        /// <summary>
        /// A value describing the South-East Direction.
        /// </summary>
        SouthEast = South | East
    }

    /// <summary>
    /// Provides extension methods for the Direction enumeration.
    /// </summary>
    public static class Directions
    {
        /// <summary>
        /// Tests if a Direction is not contradictory. That is, the Direction
        /// does not contain opposite Direction values and is not equal to 
        /// <c>Direction.None</c>.
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <returns>True if the Direction is valid, false otherwise.</returns>
        public static bool IsValid(this Direction direction)
        {
            return direction != Direction.None
                && (direction | Direction.North | Direction.South) != direction
                && (direction | Direction.East | Direction.West) != direction;
        }

        /// <summary>
        /// Returns the vertical component of the Direction.
        /// <para>
        /// The only result values of this method can be <c>Direction.North</c>,
        /// <c>Direction.South</c> or <c>Direction.None</c>, provided the Direction
        /// is valid.
        /// </para>
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <returns>The vertical Direction of the Direction</returns>
        public static Direction GetVerticalComponent(this Direction direction)
        {
            return direction & (Direction.North | Direction.South);
        }

        /// <summary>
        /// Returns the horizontal component of the Direction.
        /// <para>
        /// For example, <c>NorthWest.GetHorizontalComponent() == West</c>, 
        /// and <c>South.GetHorizontalComponent() == None</c>.
        /// </para>
        /// <para>
        /// The only result values of this method can be <c>Direction.East</c>,
        /// <c>Direction.West</c> or <c>Direction.None</c>, provided the Direction
        /// is valid.
        /// </para>
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <returns>The horizontal Direction of the Direction</returns>
        public static Direction GetHorizontalComponent(this Direction direction)
        {
            return direction & (Direction.East | Direction.West);
        }

        /// <summary>
        /// Returns the sign of the axis in the given non-combined Direction.
        /// <para>
        /// For example, <c>North.GetAxisSign() == 1</c> and <c>South.GetAxisSign() == -1</c>.
        /// </para>
        /// <para>
        /// The result of this method is only defined for Directions that are valid
        /// and not combined.
        /// </para>
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <returns>The sign of the axis in the Direction.</returns>
        public static int GetAxisSign(this Direction direction)
        {
            if (direction == Direction.None)
            {
                return 0;
            }

            return (direction & Direction.NorthWest) != Direction.None ? 1 : -1;
        }

        /// <summary>
        /// Tests whether the given direction is a combination of simple directions.
        /// <para>
        /// This method tests whether the direction contains a horizontal component 
        /// and a vertical component. The result may be wrong if the Direction is not
        /// valid.
        /// </para>
        /// </summary>
        /// <param name="direction">The Direction to test.</param>
        /// <returns>True if the Direction is a combined direction, false otherwise.</returns>
        public static bool IsCombinedDirection(this Direction direction)
        {
            return direction.GetHorizontalComponent() != Direction.None
                && direction.GetVerticalComponent() != Direction.None;
        }

        /// <summary>
        /// Returns the complement of this Direction.
        /// <para>
        /// For example, the complement of <c>Direction.NorthEast</c> is 
        /// <c>Direction.SouthWest</c>. 
        /// </para>
        /// <para>
        /// The complement of a valid, combined Direction is always valid.
        /// However, the complement for non-combined Directions results in 
        /// a non-valid Direction.
        /// </para>
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <returns>The complementing Direction</returns>
        public static Direction Complement(this Direction direction)
        {
            return direction ^ (Direction.NorthWest | Direction.SouthEast);
        }

        /// <summary>
        /// Returns the opposite of the non-combined Direction.
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <returns>The opposite Direction.</returns>
        public static Direction Opposite(this Direction direction)
        {
            Direction complement = direction.Complement();
            if (complement.GetHorizontalComponent().IsValid())
            {
                return complement.GetHorizontalComponent();
            }
            else
            {
                return complement.GetVerticalComponent();
            }
        }
    }
}