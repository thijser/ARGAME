//----------------------------------------------------------------------------
// <copyright file="LensSplitter.cs" company="Delft University of Technology">
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
    using Core;
    using UnityEngine;

    /// <summary>
    /// Splits a Laser beam into two equivalent, smaller beams that 
    /// are then emitted in separate directions.
    /// </summary>
    public class LensSplitter : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// Transform of crystal part.
        /// </summary>
        private Transform focusPoint;

        /// <summary>
        /// GameObject of the left output part.
        /// </summary>
        private GameObject outLeft;

        /// <summary>
        /// GameObject of the right output part.
        /// </summary>
        private GameObject outRight;

        /// <summary>
        /// Set to true if OnLaserHit is called during the frame (before LateUpdate).
        /// </summary>
        private bool hit = false;

        /// <summary>
        /// RGB strengths of incoming Laser beam.
        /// </summary>
        private Vector3 rgbStrengths;

        /// <summary>
        /// Find references to the LensSplitter parts during initialization.
        /// </summary>
        public void Start()
        {
            this.focusPoint = transform.parent.Find("focusPoint");
            this.outLeft = transform.parent.Find("outLeft").gameObject;
            this.outRight = transform.parent.Find("outRight").gameObject;
        }

        /// <summary>
        /// Curves the incoming laser beam towards the crystal.
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
                throw new ArgumentException("The supplied HitEventArgs object was invalid.");
            }

            Vector3 incomingDir = args.Laser.Direction;
            Vector3 incomingDirLocal = transform.worldToLocalMatrix.MultiplyVector(incomingDir);

            // Check if laser is hitting lens from correct side
            if (incomingDirLocal.z > 0.0f)
            {
                this.hit = true;
                this.rgbStrengths = args.Laser.Emitter.GetComponent<LaserProperties>().RGBStrengths;

                // Slight offset to make sure the laser passes through the surface
                Vector3 dir = this.focusPoint.position - args.Point;
                args.Laser.Extend(args.Point + (dir.normalized * 0.1f), dir);
            }
        }

        /// <summary>
        /// Updates the strengths of the left and right emitters.
        /// </summary>
        public void LateUpdate()
        {
            this.outLeft.SetActive(this.hit);
            this.outRight.SetActive(this.hit);

            // Split the incoming light between the two outputs
            this.outLeft.GetComponent<LaserProperties>().RGBStrengths = this.rgbStrengths;
            this.outRight.GetComponent<LaserProperties>().RGBStrengths = this.rgbStrengths;

            this.hit = false;
        }

        /// <summary>
        /// Returns whether or not the object is hit by a laser beam.
        /// </summary>
        /// <returns>True if the object is hit by a laser beam in the 
        /// same frame, false otherwise.</returns>
        public bool IsHit()
        {
            return this.hit;
        }
    }
}