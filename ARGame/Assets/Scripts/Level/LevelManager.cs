//----------------------------------------------------------------------------
// <copyright file="LevelManager.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Level
{
    using System.Text.RegularExpressions;
    using Network;
    using UnityEngine;
    using Logger = Core.Logger;

    /// <summary>
    /// Manages loading and changing of levels.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        /// <summary>
        /// The TiledLevelLoader used for the loading of levels.
        /// </summary>
        private LevelLoader levelLoader = new LevelLoader();

        /// <summary>
        /// Mappings from level IDs to names.
        /// </summary>
        private string[] levelMappings = null;

        /// <summary>
        /// The level GameObject.
        /// </summary>
        private GameObject level;
        
        /// <summary>
        /// Gets or sets the size of the board.
        /// </summary>
        public Vector2 BoardSize { get; set; }

        /// <summary>
        /// Gets the current level index.
        /// </summary>
        public int CurrentLevelIndex { get; private set; }

        /// <summary>
        /// Loads the level mappings.
        /// </summary>
        public void Start()
        {
            this.LoadLevelMappings();
        }

        /// <summary>
        /// Loads the next level in sequence.
        /// </summary>
        public void NextLevel()
        {
            this.LoadLevel(++this.CurrentLevelIndex);
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartLevel()
        {
            this.LoadLevel(this.CurrentLevelIndex);
        }

        /// <summary>
        /// Restarts the game, loading the first level.
        /// </summary>
        public void RestartGame()
        {
            this.LoadLevel(0);
        }

        /// <summary>
        /// Loads the level indicated by the given index.
        /// </summary>
        /// <param name="index">The index of the level to load.</param>
        public void LoadLevel(int index)
        {
            GetComponent<Logger>().NewLevel(this.CurrentLevelIndex, index);

            Debug.Log("loading level : " + index);
            if (this.level != null)
            {
                GameObject.Destroy(this.level);
            }

            this.levelLoader.BoardSize = this.BoardSize;
            if(this.levelMappings == null)
            {
                this.LoadLevelMappings();
            }

            this.level = this.levelLoader.CreateLevel("Levels/" + this.levelMappings[index]);
            this.CurrentLevelIndex = index;
            this.level.transform.SetParent(this.transform);
        }

        /// <summary>
        /// Loads the level indicated by the <see cref="LevelUpdate"/> if
        /// that level is not yet loaded. If the same level is already loaded,
        /// this method does nothing.
        /// </summary>
        /// <param name="update">The level update.</param>
        public void OnLevelUpdate(LevelUpdate update)
        {
            if (this.CurrentLevelIndex != update.NextLevelIndex ||
                this.BoardSize != update.Size)
            {
                this.BoardSize = update.Size;
                this.LoadLevel(update.NextLevelIndex);
            }
        }

        /// <summary>
        /// Loads the mappings from level IDs to names.
        /// </summary>
        private void LoadLevelMappings()
        {
            string data = (Resources.Load("Levels/Index") as TextAsset).text;
            this.levelMappings = Regex.Split(data, "\r?\n");
        }
    }
}