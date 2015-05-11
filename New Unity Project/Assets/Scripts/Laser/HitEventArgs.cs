//----------------------------------------------------------------------------
// <copyright file="HitEventArgs.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser {
    using System;
    using UnityEngine;

    /// <summary>
    /// Describes the event of a Laser beam hitting an object.
    /// </summary>
    public class HitEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="HitEventArgs"/> class.
        /// </summary>
        /// <param name="laser">The Laser beam that hit the object</param>
        /// <param name="normal">The normal of the surface that the Laser beam hit</param>
        public HitEventArgs(Laser laser, Vector3 point, Vector3 normal) {
            this.Laser = laser;
            this.Point = point;
            this.Normal = normal;
        }

        /// <summary>
        /// Gets or sets the Laser beam that hit the object.
        /// </summary>
        public Laser Laser { get; set; }

        /// <summary>
        /// Gets or sets the normal of the surface that the Laser beam hit.
        /// </summary>
        public Vector3 Normal { get; set; }

        /// <summary>
        /// Gets or sets the position on the surface that the Laser beam hit.
        /// </summary>
        public Vector3 Point { get; set; }
    }
}
