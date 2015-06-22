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
    using System.IO;
    using Network;
    using UnityEngine;
    using Vision;

    /// <summary>
    /// Manages loading and changing of levels.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        /// <summary>
        /// The TiledLevelLoader used for the loading of levels.
        /// </summary>
        private TiledLevelLoader levelLoader = new TiledLevelLoader();

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
        /// Gets or sets the scale of the underlying <see cref="IARLink"/> implementation.
        /// </summary>
        public float IARscale { get; set; }

        /// <summary>
        /// Gets the current level index.
        /// </summary>
        public int CurrentLevelIndex { get; private set; }

        /// <summary>
        /// Initializes this LevelManager and loads the first level.
        /// </summary>
        public void Start()
        {
            this.LoadLevelMappings();

            this.IARscale = 1;
            IARLink link = gameObject.GetComponent<IARLink>();
            if (link != null)
            {
                this.IARscale = link.GetScale();
            }

            this.RestartGame();
        }

        /// <summary>
        /// Loads the mappings from level IDs to names.
        /// </summary>
        private void LoadLevelMappings()
        {
            string data = (Resources.Load("Levels/Index") as TextAsset).text;

            levelMappings = data.Split(new string[] {"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);
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
            GameObject.Destroy(this.level);
            this.levelLoader.BoardSize = this.BoardSize;
            this.level = this.levelLoader.CreateLevel("Levels/" + levelMappings[index]);
            this.CurrentLevelIndex = index;
            this.level.transform.SetParent(this.transform);
            this.level.transform.localScale = this.IARscale * Vector3.one;
        }

        /// <summary>
        /// Loads the level indicated by the <see cref="LevelUpdate"/> if
        /// that level is not yet loaded. If the same level is already loaded,
        /// this method does nothing.
        /// </summary>
        /// <param name="levelup">The <see cref="LevelUpdate"/>.</param>
        public void OnLevelUpdate(LevelUpdate levelup)
        {
            if (this.CurrentLevelIndex != levelup.NextLevelIndex ||
                this.BoardSize != levelup.Size)
            {
                this.BoardSize = levelup.Size;
                this.LoadLevel(levelup.NextLevelIndex);
            }
        }

        /// <summary>
        /// Scales the level along with the board size.
        /// </summary>
        public void ScaleLevel()
        {
            Levelcomp levelcomp = this.level.GetComponent<Levelcomp>();
            float xproportions = this.BoardSize.x / levelcomp.Size.x;
            float yproportions = this.BoardSize.y / levelcomp.Size.y;
            this.level.transform.localScale = Mathf.Min(xproportions, yproportions) * Vector3.one * this.IARscale;
        }
    }
}