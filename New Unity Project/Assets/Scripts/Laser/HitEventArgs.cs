//----------------------------------------------------------------------------
// <copyright file="HitEventArgs.cs" company="Delft University of Technology">
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
    /// Describes the event of a Laser beam hitting an object.
    /// </summary>
    public class HitEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HitEventArgs"/> class.
        /// <para>
        /// The created HitEventArgs is not valid. To create a valid instance, 
        /// either use the alternative constructor, or assign a Laser instance to 
        /// the Laser property of this HitEventArgs.
        /// </para>
        /// </summary>
        public HitEventArgs()
        {
            this.Laser = null;
            this.Normal = Vector3.zero;
            this.Point = Vector3.zero;
            this.Receiver = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitEventArgs"/> class.
        /// </summary>
        /// <param name="laser">The Laser beam that hit the object.</param>
        /// <param name="point">The position where the Laser hit the object.</param>
        /// <param name="normal">The normal of the surface that the Laser beam hit.</param>
        /// <param name="receiver">The ILaserReceiver that got hit.</param>
        public HitEventArgs(LaserBeam laser, Vector3 point, Vector3 normal, ILaserReceiver receiver)
        {
            this.Laser = laser;
            this.Point = point;
            this.Normal = normal;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets or sets the Laser beam that hit the object.
        /// </summary>
        public LaserBeam Laser { get; set; }

        /// <summary>
        /// Gets or sets the normal of the surface that the Laser beam hit.
        /// </summary>
        public Vector3 Normal { get; set; }

        /// <summary>
        /// Gets or sets the position on the surface that the Laser beam hit.
        /// </summary>
        public Vector3 Point { get; set; }

        /// <summary>
        /// Gets or sets the ILaserReceiver that was hit.
        /// </summary>
        public ILaserReceiver Receiver { get; set; }

        /// <summary>
        /// Gets a value indicating whether this HitEventArgs represents a 
        /// valid hit event.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.Laser != null && this.Receiver != null;
            }
        }
    }
}
