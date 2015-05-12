﻿//----------------------------------------------------------------------------
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
        public MultiEmitter PortalEmitter { get; set; }

        /// <summary>
        /// Gets the surface normal of this Portal.
        /// <para>
        /// The Vector3 points out from the active side of the Portal.
        /// </para>
        /// </summary>
        public Vector3 SurfaceNormal
        {
            get
            {
                return this.transform.rotation * new Vector3(1, 0, 0);
            }
        }

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

            this.PortalEmitter = gameObject.AddComponent<MultiEmitter>();
        }

        /// <summary>
        /// Emits a copy of the given Laser beam at the given offset and angle,
        /// relative to this Portal.
        /// </summary>
        /// <param name="laser">The Laser beam to copy.</param>
        /// <param name="offset">The offset of the Laser beam.</param>
        /// <param name="angle">The angle from the normal of this Portal's plane.</param>
        /// <returns>The created Laser beam.</returns>
        public void EmitLaserBeam(Laser laser, Vector3 offset, Vector3 angle)
        {
            LaserEmitter emitter = this.PortalEmitter.GetEmitter(laser);
            emitter.transform.position = offset;
            Quaternion rotation = new Quaternion();
            rotation.SetFromToRotation(Vector3.forward, angle);
            emitter.transform.rotation = rotation;
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
                Vector3 translation = this.LinkedPortal.transform.position - this.transform.position;
                Quaternion rotation = new Quaternion();
                rotation.SetFromToRotation(-1 * this.SurfaceNormal, this.LinkedPortal.SurfaceNormal);
                Vector3 direction = args.Point - args.Laser.Origin;
                this.LinkedPortal.EmitLaserBeam(args.Laser, translation + args.Point, rotation * -direction.normalized);
            }
        }
    }
}
