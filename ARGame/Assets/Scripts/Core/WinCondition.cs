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
        /// Gets all targets in the level.
        /// </summary>
        public LaserTarget[] Targets { get; private set; }

        /// <summary>
        /// Gets all checkpoints in the level.
        /// </summary>
        public Checkpoint[] Checks { get; private set; }

        /// <summary>
        /// Initializes the target array.
        /// </summary>
        public void Start()
        {
            this.Targets = GameObject.FindObjectsOfType<LaserTarget>();
            this.Checks = GameObject.FindObjectsOfType<Checkpoint>();
        }

        /// <summary>
        /// Tracks if all targets have been opened.
        /// If so, moves on to the next level.
        /// </summary>
        public void LateUpdate()
        {
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
                this.SendMessageUpwards("OnLevelCompleted", new LevelUpdate(this.NextLevelIndex, Vector2.one));
            }
        }
    }
}
