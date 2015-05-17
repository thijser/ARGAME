//----------------------------------------------------------------------------
// <copyright file="AndGate.cs" company="Delft University of Technology">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Core.Emitter;
    using UnityEngine;

    /// <summary>
    /// An AND-gate that outputs a laser beam if two other beams hit it.
    /// </summary>
    public class AndGate : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// The minimal strength required to make the crystal open.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public float MinimumStrength;

        /// <summary>
        /// A variable storing whether or not a previous laser hit the gate.
        /// </summary>
        public bool hit { get; private set; }

        /// <summary>
        /// A variable storing whether or not a beam has already been
        /// created this tick.
        /// </summary>
        public bool beamcreated { get; private set; }

        /// <summary>
        /// Gets or sets the LaserEmitter used for creating new Laser beam segments.
        /// </summary>
        public MultiEmitter PassThroughEmitter { get; set; }

        /// <summary>
        /// The Start method, invoked when the scene is running.
        /// </summary>
        public void Start()
        {
            hit = false;
            beamcreated = false;
            this.PassThroughEmitter = gameObject.AddComponent<MultiEmitter>();
        }

        /// <summary>
        /// Creates a new laser beam if two existing laser beams hit
        /// the gate.
        /// </summary>
        /// <param name="sender">The sender of the event, ignored here.</param>
        /// <param name="args">The EventArgs object that describes the event.</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            if(args == null)
            {
                throw new ArgumentNullException("args");
            }

            if(!args.IsValid)
            {
                throw new ArgumentException("The HitEventArgs object supplied is invalid.");
            }

            if (this.hit && !this.beamcreated)
            {
                // Create a new ray coming out of the other side with the same direction
                // as the original ray. Forward needs to be negative, see LaserEmitter.
                var passThroughEmitter = this.PassThroughEmitter.GetEmitter(args.Laser);

                passThroughEmitter.transform.position = args.Point + (args.Laser.Direction * 0.1f);
                passThroughEmitter.transform.forward = -args.Laser.Direction;
                LaserProperties propertiesPre = args.Laser.Emitter.GetComponent<LaserProperties>();
                LaserProperties propertiesPost = passThroughEmitter.GetComponent<LaserProperties>();
                propertiesPost.RGBStrengths = propertiesPre.RGBStrengths;
                this.beamcreated = true;
            }

            if (!this.hit)
            {
                this.hit = true;
            }
        }

        /// <summary>
        /// Resets the variables for use in the next tick.
        /// </summary>
        public void LateUpdate()
        {
            this.hit = false;
            this.beamcreated = false;
        }
    }
}
