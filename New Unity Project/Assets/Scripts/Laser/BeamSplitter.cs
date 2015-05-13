//----------------------------------------------------------------------------
// <copyright file="BeamSplitter.cs" company="Delft University of Technology">
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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// An object that splits the beam into a reflection and pass-through ray.
    /// </summary>
    public class BeamSplitter : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// Gets or sets the LaserEmitter used for creating new Laser beam segments.
        /// </summary>
        public MultiEmitter PassThroughEmitter { get; set; }

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

            // Create a new ray coming out of the other side with the same direction
            // as the original ray. Forward needs to be negative, see LaserEmitter.
            var passThroughEmitter = this.PassThroughEmitter.GetEmitter(args.Laser);

            passThroughEmitter.transform.position = args.Point + (args.Laser.Direction * 0.1f);
            passThroughEmitter.transform.forward = -args.Laser.Direction;
            LaserProperties propertiesPre = args.Laser.Emitter.GetComponent<LaserProperties>();
            LaserProperties propertiesPost = passThroughEmitter.GetComponent<LaserProperties>();
            propertiesPost.RGBStrengths = propertiesPre.RGBStrengths / 2;

            // Create the second ray, reflecting off surface like a mirror
            Mirror.CreateReflection(args.Laser, args.Normal);
        }
    }
}
