//----------------------------------------------------------------------------
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
    public class BoardSizeUpdate : AbstractUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoardSizeUpdate"/> class.
        /// </summary>
        /// <param name="size">The new size of the board.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If one of the values in the supplied <see cref="Vector2"/> is not positive.
        /// </exception>
        public BoardSizeUpdate(Vector2 size)
        {
            if (size.x <= 0 || size.y <= 0)
            {
                throw new ArgumentOutOfRangeException("size", size, "The board must have positive dimensions");
            }

            this.Size = size;
        }

        /// <summary>
        /// Gets the new board size.
        /// </summary>
        public Vector2 Size { get; private set; }
    }
}
