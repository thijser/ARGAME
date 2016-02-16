//----------------------------------------------------------------------------
// <copyright file="Mirror.cs" company="Delft University of Technology">
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
    using UnityEngine;

    /// <summary>
    /// A Mirror that reflects Laser beams.
    /// </summary>
    public class Mirror : MonoBehaviour, ILaserReceiver 
    {
        /// <summary>
        /// Creates a reflection of the specified Laser beam.
        /// </summary>
        /// <returns>The reflected Laser beam segment.</returns>
        /// <param name="laser">The Laser beam.</param>
        /// <param name="surfaceNormal">The surface normal.</param>
        public static LaserBeam CreateReflection(LaserBeam laser, Vector3 surfaceNormal)
        {
            if (laser == null)
            {
                throw new ArgumentNullException("laser");
            }

            Vector3 reflection = Vector3.Reflect(laser.Direction, surfaceNormal);
            return laser.Extend(laser.Endpoint, reflection);
        }

        /// <summary>
        /// Reflects the argument Laser beam and creates a new Laser beam
        /// in the reflected direction.
        /// </summary>
        /// <param name="sender">The sender of the event, ignored here.</param>
        /// <param name="args">The EventArgs UnityEngine.Object that describes the event.</param>
        public void OnLaserHit(object sender, HitEventArgs args) 
        {
            if (args == null) 
            {
                throw new ArgumentNullException("args");
            }

            CreateReflection(args.Laser, args.Normal);
        }
    }
}
