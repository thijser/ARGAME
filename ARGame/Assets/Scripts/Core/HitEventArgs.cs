//----------------------------------------------------------------------------
// <copyright file="HitEventArgs.cs" company="Delft University of Technology">
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
        /// <param name="laser">The Laser beam that Hit the object, not null.</param>
        /// <param name="point">The position where the Laser Hit the object.</param>
        /// <param name="normal">The normal of the surface that the Laser beam Hit.</param>
        /// <param name="receiver">The ILaserReceiver that got Hit.</param>
        public HitEventArgs(LaserBeam laser, Vector3 point, Vector3 normal, ILaserReceiver receiver)
        {
            if (laser == null)
            {
                throw new ArgumentNullException("laser");
            }

            this.Laser = laser;
            this.Point = point;
            this.Normal = normal;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets the Laser beam that Hit the object.
        /// </summary>
        public LaserBeam Laser { get; private set; }

        /// <summary>
        /// Gets the normal of the surface that the Laser beam Hit.
        /// </summary>
        public Vector3 Normal { get; private set; }

        /// <summary>
        /// Gets the position on the surface that the Laser beam Hit.
        /// </summary>
        public Vector3 Point { get; private set; }

        /// <summary>
        /// Gets the ILaserReceiver that was Hit.
        /// </summary>
        public ILaserReceiver Receiver { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this HitEventArgs represents a 
        /// valid Hit event.
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
