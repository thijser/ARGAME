//----------------------------------------------------------------------------
// <copyright file="BoardResizer.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using System.Linq;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Resizes the board when a level serverUpdate is received with a new board size.
    /// </summary>
    public class BoardResizer : MonoBehaviour
    {
        /// <summary>
        /// Updates the board size with the size in the argument.
        /// </summary>
        /// <param name="level">The level update.</param>
        public void OnLevelUpdate(LevelUpdate level)
        {
            Debug.Log("Applying Board Size: " + level.Size);
            this.UpdateBoardSize(level.Size);
        }

        /// <summary>
        /// Updates the size of the playing board.
        /// </summary>
        /// <param name="size">The new board size.</param>
        /// <returns>True if the board size was updated, false if no board was found.</returns>
        public bool UpdateBoardSize(Vector2 size)
        {
            Transform board = GetComponentsInChildren<Transform>()
                .FirstOrDefault(t => t.gameObject.tag == "PlayingBoard");
            if (board != null)
            {
                Vector3 scale = new Vector3(8 * size.x, board.localScale.y, -8 * size.y);
                board.localScale = scale;
                return true;
            }

            return false;
        }
    }
}