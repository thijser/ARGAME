//----------------------------------------------------------------------------
// <copyright file="LocalMarker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Represents a marker detected in the world.
    /// <para>
    /// This class acts as a container for the position and rotation data 
    /// coming from both the remote server as well as the local IARLink 
    /// implementation.
    /// </para>
    /// </summary>
    public class LocalMarker : Marker
    {
        /// <summary>
        /// The Id of the Marker.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int MarkerId = -1;

        /// <summary>
        /// Gets or sets the local position of this marker.
        /// </summary>
        public MarkerPosition LocalPosition { get; set; }

        /// <summary>
        /// Gets the remote to local transformation matrix.
        /// <para>
        /// This matrix can be used to transform markers relative to this marker. Both the 
        /// <c>LocalPosition</c> and <c>RemotePosition</c> properties must be set for this 
        /// method to return any useful result.
        /// </para>
        /// <para>
        /// If the <c>RemotePosition</c> is not set, it is taken as the identity matrix.
        /// If the <c>LocalPosition</c> is not set, this method returns the identity matrix.
        /// </para>
        /// </summary>
        public Matrix4x4 TransformMatrix
        {
            get
            {
                if (this.LocalPosition == null)
                {
                    return Matrix4x4.identity;
                }

                if (this.RemotePosition == null)
                {
                    return this.LocalPosition.Matrix;
                }

                return this.LocalPosition.Matrix * this.RemotePosition.Matrix.inverse;
            }
        }

        /// <summary>
        /// Registers this marker.
        /// </summary>
        public void Start()
        {
            this.Id = this.MarkerId;
            this.SendMessageUpwards("OnMarkerRegister", new MarkerRegister(this));
        }

        /// <summary>
        /// Returns a string representation of this Marker.
        /// </summary>
        /// <returns>A string describing this Marker.</returns>
        public override string ToString()
        {
            return "<marker:id=" + this.Id +
                ", RemotePosition=" + this.RemotePosition +
                ", LocalPosition=" + this.LocalPosition +
                ", ObjectRotation=" + this.ObjectRotation + ">";
        }
    }
}