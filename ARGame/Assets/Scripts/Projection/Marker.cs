//----------------------------------------------------------------------------
// <copyright file="Marker.cs" company="Delft University of Technology">
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
    public class Marker : MonoBehaviour
    {
        /// <summary>
        /// The ID of the Marker.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int MarkerID = -1;

        /// <summary>
        /// Gets or sets the ID of this Marker.
        /// <para>
        /// If the ID is already set to a valid ID, the setter does nothing.
        /// </para>
        /// </summary>
        public int ID
        {
            get
            {
                return this.MarkerID;
            }

            set
            {
                if (this.MarkerID < 0)
                {
                    this.MarkerID = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the remote position of this marker.
        /// </summary>
        public MarkerPosition RemotePosition { get; set; }

        /// <summary>
        /// Gets or sets the local position of this marker.
        /// </summary>
        public MarkerPosition LocalPosition { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the object on this marker.
        /// </summary>
        public float ObjectRotation { get; set; }

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
            this.SendMessageUpwards("OnMarkerRegister", new MarkerRegister(this));
        }

        /// <summary>
        /// Updates the position of this Marker relative to the given parent Marker.
        /// </summary>
        /// <param name="level">The parent Marker, not null.</param>
        [Obsolete("Use UpdatePosition(Matrix4x4) for improved performance.")]
        public void UpdatePosition(Marker level)
        {
            if (level == null)
            {
                throw new ArgumentNullException("level");
            }


            this.UpdatePosition(level.TransformMatrix);
        }

        /// <summary>
        /// Updates the position of the Marker using the provided transformation matrix.
        /// <para>
        /// The argument matrix represents the linear transformation from the remote coordinate
        /// system to the local coordinate system.
        /// </para>
        /// </summary>
        /// <param name="transformMatrix">The remote to local transformation matrix.</param>
        public void UpdatePosition(Matrix4x4 transformMatrix)
        {
            if (this.RemotePosition != null)
            {
                Vector3 originalRotation = this.transform.localEulerAngles;
                this.transform.SetFromMatrix(transformMatrix * this.RemotePosition.Matrix);
                this.RotateChildren(originalRotation - this.transform.localEulerAngles);
            }
        }

        /// <summary>
        /// Rotates the children of this Marker by the given rotation.
        /// </summary>
        /// <param name="rotation">The rotation in euler angles.</param>
        public void RotateChildren(Vector3 rotation)
        {
            Quaternion inverse = Quaternion.Euler(rotation);
            foreach (Transform t in this.GetComponentsInChildren<Transform>())
            {
                t.localRotation = t.localRotation * inverse;
            }
        }
            
        /// <summary>
        /// Returns a string representation of this Marker.
        /// </summary>
        /// <returns>A string describing this Marker.</returns>
        public override string ToString()
        {
            return "<marker:id=" + this.ID +
                ", RemotePosition=" + this.RemotePosition +
                ", LocalPosition=" + this.LocalPosition +
                ", ObjectRotation=" + this.ObjectRotation + ">";
        }
    }
}