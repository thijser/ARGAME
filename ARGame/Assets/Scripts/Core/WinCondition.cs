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
    using UnityEngine;
    using SysDebug = System.Diagnostics.Debug;

    /// <summary>
    /// A class that tracks if the level has been won.
    /// </summary>
    public class WinCondition : MonoBehaviour
    {
        /// <summary>
        /// The index of the next level.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int NextLevelIndex;

        /// <summary>
        /// All targets in the level.
        /// </summary>
        private LaserTarget[] targets;

        /// <summary>
        /// All checkpoints in the level.
        /// </summary>
        private Checkpoint[] checks;

        /// <summary>
        /// Initializes the target array.
        /// </summary>
        public void Start()
        {
            this.targets = GameObject.FindObjectsOfType<LaserTarget>();
            this.checks = GameObject.FindObjectsOfType<Checkpoint>();

            if (this.NextLevelIndex < 0 || this.NextLevelIndex >= Application.levelCount)
            {
                Debug.LogError("NextLevelIndex is set to " + this.NextLevelIndex +
                    ", but should be between 0 and " + Application.levelCount);
            }
        }

        /// <summary>
        /// Tracks if all targets have been opened.
        /// If so, moves on to the next level.
        /// </summary>
        public void LateUpdate()
        {
            // If the length of targets is 0, the level cannot be completed.
            // As such, this should never happen in scenes where this script exists.
            SysDebug.Assert(this.targets.Length > 0);
            bool win = Array.TrueForAll(
                this.targets, 
                t => t.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opened"));

            win = win && Array.TrueForAll(
                this.checks,
                t => t.Hit);

            Array.ForEach(this.targets, t => t.Reset());
            Array.ForEach(this.checks, t => t.Reset());

            if (win)
            {
                Application.LoadLevel(this.NextLevelIndex);
            }
        }
    }
}
