namespace Core
{
    using System;
    using System.Collections;
    using Level;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Provides a win screen that displays a message that the level was completed, as well as 
    /// </summary>
    public class WinScreen : MonoBehaviour {
        /// <summary>
        /// The time in seconds that the win screen is visible.
        /// </summary>
        public const float TransitionTime = 5;

        /// <summary>
        /// Gets the main text.
        /// </summary>
        public TextMesh MainText { get; private set; }

        /// <summary>
        /// Gets the subtitle text.
        /// </summary>
        public TextMesh SubText { get; private set; }

        /// <summary>
        /// Gets the orthographic camera.
        /// </summary>
        public Camera OrthoCamera { get; private set; }

        /// <summary>
        /// Gets or sets the next level to go to after displaying the win screen.
        /// </summary>
        public int NextLevel { get; private set; }

        /// <summary>
        /// Gets whether the win screen is being displayed.
        /// </summary>
        public bool ShowingWinScreen
        {
            get { return this.OrthoCamera.enabled; }
            private set { this.OrthoCamera.enabled = value; }
        }

        /// <summary>
        /// Gets the root <see cref="GameObject"/> from the scene.
        /// </summary>
        private GameObject RootObject
        {
            get { return GameObject.Find("MetaWorld") ?? GameObject.Find("RemoteController");  }
        }

        /// <summary>
        /// Gets the <see cref="LevelManager"/>.
        /// </summary>
        private LevelManager LevelManager
        {
            get { return this.RootObject.GetComponent<LevelManager>(); }
        }

        /// <summary>
        /// Gets the current level index.
        /// </summary>
        public int CurrentLevel
        {
            get { return this.LevelManager.CurrentLevelIndex; }
        }

        /// <summary>
        /// Initializes this <see cref="WinScreen"/> instance.
        /// </summary>
        public void Start() {
            this.MainText = transform.Find("MainText").GetComponent<TextMesh>();
            this.SubText = transform.Find("SubText").GetComponent<TextMesh>();
            this.OrthoCamera = transform.Find("Camera").GetComponent<Camera>();

            // Initially don't draw the win screen
            this.ShowingWinScreen = false;
        }

        /// <summary>
        /// Displays the win screen using the provided information.
        /// </summary>
        /// <param name="timeSpent">The time it took to complete the level.</param>
        /// <param name="nextLevel">The next level to go to.</param>
        public void FinishLevel(int timeSpent, int nextLevel) {
            this.MainText.text = "You completed level " + this.CurrentLevel + " in " + this.FormatSeconds(timeSpent) + " minutes !";
            this.SubText.text = "Going to the next level in " + Convert.ToInt32(TransitionTime) + " seconds...";
            this.ShowingWinScreen = true;
            this.StartCoroutine(this.LoadNextLevel(nextLevel));
        }

        /// <summary>
        /// Handles the given <see cref="LevelUpdate"/>.
        /// </summary>
        /// <param name="level">The <see cref="LevelUpdate"/>.</param>
        public void OnLevelUpdate(LevelUpdate level)
        {
            this.FinishLevel(level.TimeTaken, level.NextLevelIndex);
        }

        /// <summary>
        /// Formats an amount of seconds to a time.
        /// </summary>
        /// <param name="time">The amount of seconds.</param>
        /// <returns>The formatted time.</returns>
        private string FormatSeconds(int time)
        {
            if (time < 0)
            {
                throw new ArgumentOutOfRangeException("time", time, "The time must be positive");
            }

            int seconds = time % 60;
            int minutes = (time / 60) % 60;
            int hours = (time / 3600);
            string result = (hours > 0 ? hours.ToString() + ':' + minutes.ToString("D2") : minutes.ToString()) +
                ':' + seconds.ToString("D2");

            return result;
        }

        /// <summary>
        /// Loads the next level after waiting for the amount of seconds indicated by <c>TransitionTime</c>.
        /// <para>
        /// This method should be started using the <c>StartCoroutine</c> method.
        /// </para>
        /// </summary>
        /// <param name="level">The next level to load.</param>
        /// <returns>The <see cref="IEnumerator"/> used for waiting.</returns>
        private IEnumerator LoadNextLevel(int level)
        {
            yield return new WaitForSeconds(TransitionTime);

            this.LevelManager.LoadLevel(level);
            this.ShowingWinScreen = false;
        }
    }
}