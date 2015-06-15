﻿//----------------------------------------------------------------------------
// <copyright file="BoardSizeUpdate.cs" company="Delft University of Technology">
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
    /// Update sent by the server whenever the board size is changed.
    /// <para>
    /// Usually, this message is sent only once just after the client connects.
    /// </para>
    /// </summary>
    public class LevelUpdate : AbstractUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoardSizeUpdate"/> class.
        /// </summary>
        /// <param name="index">The index of the next level.</param>
        /// /// <param name="size">The new size of the board.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If one of the values in the supplied <see cref="Vector2"/> is not positive,
        /// or if the supplied level index is not positive.
        /// </exception>
        public LevelUpdate(int index, Vector2 size)
        {
            this.Type = UpdateType.Level;
            this.ID = -1;
            if (size.x <= 0 || size.y <= 0)
            {
                throw new ArgumentOutOfRangeException("size", size, "The board must have positive dimensions");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "The index of the next level must be a positive integer.");
            }

            this.Size = size;
            this.NextLevelIndex = index;
        }

        /// <summary>
        /// Gets the new board size.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// Gets the index of the next level.
        /// </summary>
        public int NextLevelIndex { get; private set; }

        public override int GetHashCode()
        {
            return (this.GetType().GetHashCode() * this.NextLevelIndex) ^ this.Size.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            LevelUpdate that = obj as LevelUpdate;
            if (that == null)
            {
                return false;
            }
            return this.NextLevelIndex == that.NextLevelIndex
                && this.Size == that.Size;
        }

        public override string ToString()
        {
            return "<LevelUpdate[NextLevelIndex=" + this.NextLevelIndex + ", Size=" + this.Size + "]>";
        }
    }
}
