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
    /// Resizes the board when a level update is received with a new board size.
    /// </summary>
    public class BoardResizer : MonoBehaviour
    {
        /// <summary>
        /// Updates the board size with the size in the argument
        /// if the argument is a LevelUpdate instance. 
        /// <para>
        /// If the argument is not a LevelUpdate instance, this method
        /// does nothing.
        /// </para>
        /// </summary>
        /// <param name="update">The server update.</param>
        public void OnServerUpdate(AbstractUpdate update)
        {
            LevelUpdate level = update as LevelUpdate;
            if (level != null)
            {
                Debug.Log("Applying Board Size: " + level.Size);
                this.UpdateBoardSize(level.Size);
            }
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
				Vector3 scale = new Vector3(-size.x, (size.x+size.y)/2, size.y);
                board.localScale = scale;
                return true;
            }

            return false;
        }
    }
}