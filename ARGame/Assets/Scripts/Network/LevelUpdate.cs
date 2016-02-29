//----------------------------------------------------------------------------
// <copyright file="LevelUpdate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using UnityEngine;

    /// <summary>
    /// UpdatePosition sent by the server whenever the board size is changed.
    /// <para>
    /// Usually, this message is sent only once just after the client connects.
    /// </para>
    /// </summary>
    public class LevelUpdate : AbstractUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelUpdate"/> class.
        /// </summary>
        /// <param name="index">The index of the next level.</param>
        /// <param name="size">The new size of the board.</param>
        /// <param name="time">The time it took to beat the previous level, in seconds.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If one of the values in the supplied <see cref="Vector2"/> is not positive,
        /// or if the supplied level index or time is not positive.
        /// </exception>
        public LevelUpdate(int index, Vector2 size, int time)
        {
            this.Type = UpdateType.UpdateLevel;
            this.Id = -1;
            if (size.x <= 0 || size.y <= 0)
            {
                throw new ArgumentOutOfRangeException("size", size, "The board must have positive dimensions");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "The index of the next level must be a positive integer.");
            }

            if (time < 0)
            {
                throw new ArgumentOutOfRangeException("time", time, "The time taken must be a positive integer.");
            }

            this.Size = size;
            this.NextLevelIndex = index;
            this.TimeTaken = time;
        }

        /// <summary>
        /// Gets the new board size.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// Gets the index of the next level.
        /// </summary>
        public int NextLevelIndex { get; private set; }

        /// <summary>
        /// Gets the time it took to beat the previous level.
        /// </summary>
        public int TimeTaken { get; private set; }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>The hash code for this object.</returns>
        public override int GetHashCode()
        {
            return (this.GetType().GetHashCode() * this.NextLevelIndex) ^ this.Size.GetHashCode();
        }

        /// <summary>
        /// Tests whether this object is equal to the argument.
        /// </summary>
        /// <param name="obj">The object to test for equality.</param>
        /// <returns>True if this object is equal to the argument, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            LevelUpdate that = obj as LevelUpdate;
            return this.NextLevelIndex == that.NextLevelIndex
                && this.Size == that.Size;
        }

        /// <summary>
        /// Returns a string representation of this LevelUpdate.
        /// </summary>
        /// <returns>A string describing this LevelUpdate.</returns>
        public override string ToString()
        {
            return "<LevelUpdate[NextLevelIndex=" + this.NextLevelIndex + ", Size=" + this.Size + "]>";
        }
    }
}
