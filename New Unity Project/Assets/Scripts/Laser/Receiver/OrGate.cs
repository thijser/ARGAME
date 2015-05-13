//----------------------------------------------------------------------------
// <copyright file="OrGate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser.Receiver
{
    /// <summary>
    /// An OR-gate that outputs a laser beam if another beam hits it.
    /// </summary>
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A script designed to implement the functionality of the OR-gate.
    /// </summary>
    public class OrGate : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// A variable storing whether or not a beam has already been
        /// created this tick.
        /// </summary>
        private bool beamcreated = false;

        /// <summary>
        /// Creates the resulting beam.
        /// </summary>
        /// <returns>The resulting Laser beam segment.</returns>
        /// <param name="laser">The Laser beam.</param>
        public LaserBeam CreateBeam(LaserBeam laser)
        {
            if (laser == null)
            {
                throw new ArgumentNullException("laser");
            }

            return laser.Extend(transform.position, laser.Direction); 
        }

        /// <summary>
        /// Creates a new laser beam if a different beam hits
        /// the gate.
        /// </summary>
        /// <param name="sender">The sender of the event, ignored here.</param>
        /// <param name="args">The EventArgs object that describes the event.</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!this.beamcreated)
            {
                this.CreateBeam(args.Laser);
                this.beamcreated = true;
            }
        }

        /// <summary>
        /// Resets the variables for use in the next tick.
        /// </summary>
        public void LateUpdate()
        {
            this.beamcreated = false;
        }
    }
}
