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
    using System.Collections;
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
            this.StartCoroutine(this.UpdateBoardSize(level.Size));
        }

        /// <summary>
        /// Updates the size of the playing board.
        /// </summary>
        /// <param name="size">The new board size.</param>
        /// <returns>True if the board size was updated, false if no board was found.</returns>
        public IEnumerator UpdateBoardSize(Vector2 size)
        {
            yield return new WaitForEndOfFrame();
            Transform board = this.GetComponentInChildren<Board>().transform.parent;
            if (board != null)
            {
                board.gameObject.SetActive(true);
                Vector3 scale = new Vector3(-size.x, board.localScale.y, size.y);
                board.localScale = scale;
            }
            else
            {
                Debug.LogError("no board found");
            }
        }
    }
}