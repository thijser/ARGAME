//----------------------------------------------------------------------------
// <copyright file="WinCondition.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using Core.Receiver;
    using Network;
    using UnityEngine;
    using Level;

    /// <summary>
    /// A class that tracks if the level has been won.
    /// </summary>
    public class WinCondition : MonoBehaviour
    {
        /// <summary>
        /// Gets all targets in the level.
        /// </summary>
        public LaserTarget[] Targets { get; private set; }

        /// <summary>
        /// Gets all checkpoints in the level.
        /// </summary>
        public Checkpoint[] Checks { get; private set; }

        /// <summary>
        /// Reference to LevelManager component.
        /// </summary>
        private LevelManager levelManager;

        /// <summary>
        /// Find any initial targets and checkpoints.
        /// </summary>
        public void Start()
        {
            this.Targets = GameObject.FindObjectsOfType<LaserTarget>();
            this.Checks = GameObject.FindObjectsOfType<Checkpoint>();

            this.levelManager = gameObject.GetComponent<LevelManager>();
        }

        /// <summary>
        /// Tracks if all targets have been opened and all checkpoints have been passed.
        /// If so, moves on to the next level.
        /// </summary>
        public void LateUpdate()
        {
            this.Targets = GameObject.FindObjectsOfType<LaserTarget>();
            this.Checks = GameObject.FindObjectsOfType<Checkpoint>();

            if (this.Targets.Length == 0 && this.Checks.Length == 0)
            {
                return;
            }

            bool win = Array.TrueForAll(
                this.Targets, 
                t => t.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opened"));

            win = win && Array.TrueForAll(
                this.Checks,
                t => t.Hit);

            Array.ForEach(this.Targets, t => t.Reset());
            Array.ForEach(this.Checks, t => t.Reset());

            if (win)
            {
                this.SendMessageUpwards("OnLevelCompleted", new LevelUpdate(this.levelManager.CurrentLevelIndex + 1, Vector2.one));
            }
        }
    }
}
