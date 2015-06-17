//----------------------------------------------------------------------------
// <copyright file="MarkerHolder.cs" company="Delft University of Technology">
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Network;
    using UnityEngine;

    /// <summary>
    /// A class that handles marker registration and updates positions.
    /// </summary>
    public class MarkerHolder : MonoBehaviour
    {
        /// <summary>
        /// Scale of the object.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public float Scale = 1;

        /// <summary>
        /// Collection of all registered to this class. 
        /// </summary>
        private Dictionary<int, Marker> markerTable = new Dictionary<int, Marker>();

        /// <summary>
        /// How long are we willing to wait after losing track of a marker. 
        /// </summary>
        private long patience = 1; // 1000 * 10000; // 1000 milliseconds 

        /// <summary>
        /// Gets or sets the central level marker, this should be visible. 
        /// </summary>
        public Marker Parent { get; set; }

        /// <summary>
        /// Registers a new marker.
        /// <para>
        /// The <c>register</c> argument should not be null or contain a null Marker.
        /// </para>
        /// <para>
        /// This method adds the indicated marker as a child to the parent marker, or sets
        /// the marker as parent marker if no parent marker existed yet.
        /// </para>
        /// </summary>
        /// <param name="register">The marker register parameter that registers the new marker.</param>
        public void OnMarkerRegister(MarkerRegister register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }

            if (register.RegisteredMarker == null)
            {
                throw new ArgumentException("Invalid marker", "register");
            }

            this.markerTable.Add(register.RegisteredMarker.ID, register.RegisteredMarker);
        }

        /// <summary>
        /// Gets a marker by ID.
        /// </summary>
        /// <returns>The Marker.</returns>
        /// <param name="id">The ID.</param>
        /// <exception cref="KeyNotFoundException">If the marker is not (yet) registered.</exception>
        public Marker GetMarker(int id)
        {
            if (this.markerTable.ContainsKey(id))
            {
                return this.markerTable[id];
            }
            else
            {
                throw new KeyNotFoundException("this marker is not registered");
            }
        }

        /// <summary>
        /// Updates the position of all markers.
        /// </summary>
        public void Update()
        {
            if (this.Parent == null || this.Parent.LocalPosition == null)
            {
                Debug.Log("No marker is visible");
                return;
            }

            Matrix4x4 boardToLocal = this.Parent.RemotePosition.Matrix.inverse;
            Matrix4x4 localToMeta = this.Parent.LocalPosition.Matrix;
            Matrix4x4 trans = localToMeta * boardToLocal;

            GameObject tmp = new GameObject("tmp");
            tmp.transform.SetFromMatrix(trans);
            tmp.transform.LogAs("Board Projection (" + this.Parent.ID + ")");
            GameObject.Destroy(tmp);

            foreach (Marker marker in this.markerTable.Values)
            {
                marker.UpdatePosition(trans);
            }
        }

 

        /// <summary>
        /// Called whenever a marker is seen by the detector.
        /// <para>
        /// The <c>position</c> argument should not be null.
        /// </para>
        /// <para>
        /// This method updates the local position of the marker, and possibly changes the parent of 
        /// all markers to the indicated marker if the current parent is no longer visible.
        /// </para>
        /// </summary>
        /// <param name="position">The marker position, not null.</param>
        /// <exception cref="ArgumentNullException">If <c>position == null</c>.</exception>
        public void OnMarkerSeen(MarkerPosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            Marker marker = this.GetMarker(position.ID);
            this.SelectParent(marker);
            marker.LocalPosition = position;
        }

        /// <summary>
        /// Sees if the marker is more suited for being the level marker then the old marker. 
        /// If updatedMarker has been seen more recently then the parent+patience and the updateMarker is complete then replace.
        /// </summary>
        /// <param name="updatedMarker">The new parent Marker, not null.</param>
        public void SelectParent(Marker updatedMarker)
        {
            if (updatedMarker == null)
            {
                throw new ArgumentNullException("updatedMarker");
            }

            if (updatedMarker.LocalPosition == null)
            {
                return;
            }

            if (this.Parent == null || this.Parent.LocalPosition == null || this.Parent.LocalPosition.TimeStamp.Ticks + this.patience < updatedMarker.LocalPosition.TimeStamp.Ticks)
            {
                if (updatedMarker.LocalPosition != null && updatedMarker.RemotePosition != null)
                {
                    this.Parent = updatedMarker;
                }
            }
        }

        /// <summary>
        /// Called whenever a RotationUpdate is received from the remote server.
        /// <para>
        /// The <c>update</c> argument should not be null. This method will update the rotation 
        /// of the object referenced by the update to reflect the change in rotation.
        /// </para>
        /// </summary>
        /// <param name="update">The rotation update, not null.</param>
        public void OnRotationUpdate(RotationUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            this.GetMarker(update.ID).ObjectRotation = update.Rotation;
        }

        /// <summary>
        /// Updates the location of the marker based on the remote position.
        /// <para>
        /// The <c>update</c> argument should not be null. The marker with the ID referenced by the update should
        /// be registered previously using the <c>OnMarkerRegister(...)</c> method, otherwise this method logs a warning
        /// and returns without affecting any Markers. When the marker is registered, the Marker's remote position is set to 
        /// a <see cref="MarkerPosition"/> object based on the argument.
        /// </para>
        /// </summary>
        /// <param name="update">The <see cref="PositionUpdate"/>, not null.</param>
        public void OnPositionUpdate(PositionUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            try
            {
                Vector3 position = update.Coordinate;
                position.y *= -1;
                this.GetMarker(update.ID).RemotePosition = new MarkerPosition(update);
            }
            catch (KeyNotFoundException ex)
            {
                // This should never happen on well-designed levels.
                // We log a warning message stating this, and ignore it.
                // We do this because we do not want to risk throwing an exception
                // to Unity's message system.
                Debug.LogWarning("Received PositionUpdate of unknown Marker: " + ex);
            }
        }
    }
}
