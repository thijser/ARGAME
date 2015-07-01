//----------------------------------------------------------------------------
// <copyright file="Board.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System;
using UnityEngine;

/// <summary>
/// The playing board for the game.
/// </summary>
public class Board : MonoBehaviour
{
    /// <summary>
    /// The height of the board.
    /// </summary>
    public const float Height = 0.2f;

    /// <summary>
    /// Gets the current board size as a <see cref="Vector2"/>.
    /// </summary>
    public Vector2 BoardSize { get; private set; }

    /// <summary>
    /// Resizes the board to the provided size.
    /// </summary>
    /// <param name="newSize">The new board size.</param>
    public void Resize(Vector2 newSize)
    {
        if (newSize.x <= 0 || newSize.y <= 0)
        {
            throw new ArgumentOutOfRangeException("newSize", newSize, "Dimensions must be positive");
        }

        this.transform.localScale = new Vector3(-newSize.x, Height, newSize.y);
        this.BoardSize = newSize;
    }
}
