//----------------------------------------------------------------------------
// <copyright file="OrGate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Receiver
{
    /// <summary>
    /// An OR-gate that outputs a laser beam if another beam hits it.
    /// </summary>
    using System;
    using Core.Emitter;
    using UnityEngine;

    /// <summary>
    /// A script designed to implement the functionality of the OR-gate.
    /// </summary>
    public class OrGate : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// Gets a value indicating whether or not 
        /// a beam has already been created this tick.
        /// </summary>
        public bool BeamCreated { get; private set; }

        /// <summary>
        /// Gets or sets the LaserEmitter used for creating new Laser beam segments.
        /// </summary>
        public MultiEmitter PassThroughEmitter { get; set; }

        /// <summary>
        /// The Start method, invoked when the scene is running.
        /// </summary>
        public void Start()
        {
            this.BeamCreated = false;
            this.PassThroughEmitter = gameObject.AddComponent<MultiEmitter>();
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

            if (!args.IsValid)
            {
                throw new ArgumentException("The HitEventArgs object was not valid.");
            }

            if (!this.BeamCreated)
            {
                // Create a new ray coming out of the other side with the same direction
                // as the original ray. Forward needs to be negative, see LaserEmitter.
                var passThroughEmitter = this.PassThroughEmitter.GetEmitter(args.Laser);

                passThroughEmitter.transform.position = args.Point + (args.Laser.Direction * 0.1f);
                passThroughEmitter.transform.forward = -args.Laser.Direction;
                LaserProperties propertiesPre = args.Laser.Emitter.GetComponent<LaserProperties>();
                LaserProperties propertiesPost = passThroughEmitter.GetComponent<LaserProperties>();
                propertiesPost.RGBStrengths = propertiesPre.RGBStrengths;
                this.CreateBeam(args.Laser);
                this.BeamCreated = true;
            }
        }

        /// <summary>
        /// Resets the variables for use in the next tick.
        /// </summary>
        public void LateUpdate()
        {
            this.BeamCreated = false;
        }
    }
}
