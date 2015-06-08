//----------------------------------------------------------------------------
// <copyright file="Checkpoint.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Receiver
{
    using System;
    using System.Collections.Generic;
    using Core.Emitter;
    using UnityEngine;

    /// <summary>
    /// An object that verifies whether or not a laser beam has hit it.
    /// All checkpoints have to be hit in order to beat the level.
    /// </summary>
    public class Checkpoint : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// Gets or sets the LaserEmitter used for creating new Laser beam segments.
        /// </summary>
        public MultiEmitter PassThroughEmitter { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the checkpoint has been hit.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// Initialize by creating emitter object.
        /// </summary>
        public void Start()
        {
            this.PassThroughEmitter = gameObject.AddComponent<MultiEmitter>();
        }

        /// <summary>
        /// Reflects the argument Laser beam and emits a new Laser beam that
        /// passes through.
        /// </summary>
        /// <param name="sender">The sender of the event, ignored here.</param>
        /// <param name="args">The EventArgs object that describes the event.</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!args.IsValid)
            {
                throw new ArgumentException("The supplied HitEventArgs object is invalid.");
            }

            // Create a new ray coming out of the other side with the same direction
            // as the original ray. Forward needs to be negative, see LaserEmitter.
            var passThroughEmitter = this.PassThroughEmitter.GetEmitter(args.Laser);
            this.Hit = true;

            passThroughEmitter.transform.position = args.Point + (args.Laser.Direction * 0.1f);
            passThroughEmitter.transform.forward = -args.Laser.Direction;
            LaserProperties propertiesPre = args.Laser.Emitter.GetComponent<LaserProperties>();
            LaserProperties propertiesPost = passThroughEmitter.GetComponent<LaserProperties>();
            propertiesPost.RGBStrengths = propertiesPre.RGBStrengths;
            this.CreateBeam(args.Laser);
        }

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
        /// Resets hit state at the end of every frame.
        /// </summary>
        public void LateUpdate()
        {
            this.Hit = false;
        }
    }
}
