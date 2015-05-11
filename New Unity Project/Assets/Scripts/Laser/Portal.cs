//----------------------------------------------------------------------------
// <copyright file="Portal.cs" company="Delft University of Technology">
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
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// A Portal that shoots any incoming Laser beams out through another Portal instance
    /// linked to this one.
    /// </summary>
    public class Portal : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// The Portal this Portal is linked to.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Portal LinkedPortal;

        /// <summary>
        /// Gets or sets the LaserEmitter used for creating new Laser beam segments.
        /// </summary>
        public LaserEmitter PortalEmitter { get; set; }

        /// <summary>
        /// Verifies the LinkedPortal property. Called by Unity when the Portal
        /// is placed into the world.
        /// </summary>
        public void Start()
        {
            if (this.LinkedPortal == null)
            {
                Debug.LogError("No Linked Portal set.");
            }

            // FIXME: Should be replaced with a scalable solution.
            //        This solution only supports one Laser type at a time.
            this.PortalEmitter = this.gameObject.AddComponent<LaserEmitter>();
            this.PortalEmitter.LineRenderer = this.gameObject.AddComponent<LineRenderer>();
        }

        /// <summary>
        /// Emits a copy of the given Laser beam at the given offset and angle,
        /// relative to this Portal.
        /// </summary>
        /// <param name="laser">The Laser beam to copy.</param>
        /// <param name="offset">The offset of the Laser beam.</param>
        /// <param name="angle">The angle from the normal of this Portal's plane.</param>
        /// <returns>The created Laser beam.</returns>
        public Laser EmitLaserBeam(Laser laser, Vector3 offset, Vector3 angle)
        {
            Vector3 position = transform.position + offset;
            Vector3 direction = transform.eulerAngles + angle;
            Laser copy = new Laser(position, direction, null);
            copy.Create();
            return copy;
        }

        /// <summary>
        /// Emits a Laser beam with the same properties from the LinkedPortal's plane.
        /// </summary>
        /// <param name="sender">The sender of this event, not used.</param>
        /// <param name="args">The HitEventArgs that describes the event, not null</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!args.IsValid)
            {
                throw new ArgumentException("The HitEventArgs object is invalid.");
            }

            if (this.LinkedPortal != null)
            {
                Vector3 relativePosition = args.Point - this.transform.position;
                Vector3 relativeRotation = args.Normal - args.Laser.Direction;
                this.LinkedPortal.EmitLaserBeam(args.Laser, relativePosition, relativeRotation);
            }
        }
    }
}