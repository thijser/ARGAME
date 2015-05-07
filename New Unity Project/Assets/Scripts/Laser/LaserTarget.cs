//----------------------------------------------------------------------------
// <copyright file="LaserTarget.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser
{
    using System;
    using UnityEngine;

    /// <summary>
    /// A Laser target that loads the next level when hit with a Laser beam.
    /// </summary>
    public class LaserTarget : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// Gets a value indicating whether the target is fully opened.
        /// </summary>
        public bool FullyOpened { get; private set; }

		/// <summary>
		/// The name of the next level.
		/// </summary>
		public string NextLevelName;

        /// <summary>
        /// Consumes the Laser beam and opens the target one step.
        /// </summary>
        /// <param name="sender">The object that sent this event</param>
        /// <param name="args">The arguments that describe the event</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            this.AnimateStep();
            if (this.FullyOpened)
            {
                this.LoadNextLevel();
            }
        }

        /// <summary>
        /// Animates the laser target one step.
        /// </summary>
        public void AnimateStep()
        {
            // TODO: Animate the laser target
            Debug.LogError("Animation is not yet supported");
        }

        /// <summary>
        /// Loads the next level.
        /// </summary>
        public void LoadNextLevel()
        {
			Application.LoadLevel(this.NextLevelName);
        }
    }
}
