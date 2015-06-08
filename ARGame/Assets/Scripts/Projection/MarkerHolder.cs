//----------------------------------------------------------------------------
// <copyright file="PositionUpdater.cs" company="Delft University of Technology">
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

            if (this.Parent == null)
            {
                this.Parent = register.RegisteredMarker;
            }
            else
            {
                register.RegisteredMarker.transform.parent = this.Parent.transform;
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
                return;
            }

            foreach (Marker marker in this.markerTable.Values)
            {

                marker.UpdatePosition(this.Parent);
            }
        }

        /// <summary>
        /// Receives and handles all server updates.
        /// </summary>
        /// <param name="update">The update to be handled.</param>
        public void OnServerUpdate(AbstractUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            if (update.Type == UpdateType.UpdatePosition)
            {
                this.OnPositionUpdate(update as PositionUpdate);
            }
            else if (update.Type == UpdateType.UpdateRotation)
            {
                this.OnRotationUpdate(update as RotationUpdate);
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
            marker.LocalPosition = position;
            if (this.Parent.LocalPosition != null && 
                this.Parent.LocalPosition.TimeStamp.Ticks + this.patience < position.TimeStamp.Ticks)
            {
                this.Reparent(marker);
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
        /// Updates the position of the given Marker to reflect the current world state.
        /// <para>
        /// The <c>target</c> argument should not be null. If the <c>target</c> is the parent 
        /// marker, then this method behaves as if <c>UpdateParentPosition(target)</c> was invoked.
        /// Otherwise, this method behaves as if <c>UpdateChildPosition(target)</c> was invoked.
        /// </para>
        /// </summary>
        /// <param name="target">The target Marker, not null.</param>
        public void UpdatePosition(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (target == this.Parent)
            {
                this.UpdateParentPosition(target);
            }
            else
            {
                this.UpdateChildPosition(target);
            }
        }

        /// <summary>
        /// Updates the Marker position as if the supplied marker is a parent marker.
        /// <para>
        /// The <c>target</c> argument should not be null, and should have a valid local
        /// and remote position.
        /// </para>
        /// pre: target is the parent marker, target has a local position 
        /// post: target has been placed on local position with it's rotation yet to be experimentally determined. 
        /// </summary>
        /// <param name="target">The supplied target Marker.</param>
        public void UpdateParentPosition(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (target.LocalPosition == null)
            {
                throw new ArgumentException("Invalid LocalPosition in target", "target");
            }

            if (target.RemotePosition == null)
            {
                throw new ArgumentException("Invalid RemotePosition in target", "target");
            }

            target.gameObject.transform.position = target.LocalPosition.Position;
            Vector3 localrotation = target.LocalPosition.Rotation.eulerAngles;
            Vector3 remoterotation = target.RemotePosition.Rotation.eulerAngles;
            Vector3 finalrotation = localrotation - remoterotation;
            target.gameObject.transform.rotation = Quaternion.Euler(finalrotation);
        }

        /// <summary>
        /// Updates the Marker position as if the supplied Marker is a child marker.
        /// <para>
        /// The <c>target</c> argument should not be null, and the current parent should exist and have 
        /// a remote position. If the target does not have a remote position set, this method does nothing.
        /// Otherwise, this method updates the position relative to the parent position.
        /// </para> 
        /// </summary>
        /// <param name="target">The supplied target, not null.</param>
        public void UpdateChildPosition(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (this.Parent.RemotePosition == null)
            {
                throw new InvalidOperationException("Parent has no remote position");
            }

            if (target.RemotePosition != null)
            {
                target.gameObject.transform.position = target.RemotePosition.Position - this.Parent.RemotePosition.Position;
                //// TODO: If mirrored then swap operation params.
            }
        }

        /// <summary>
        /// Changes the parent to the given target Marker.
        /// <para>
        /// The <c>target</c> argument should not be null and should have a remote position. This method
        /// will translate all markers to relative positions of the target marker.
        /// </para>
        /// </summary>
        /// <param name="target">The new parent Marker, not null.</param>
        public void Reparent(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (target.RemotePosition == null)
            {
                throw new ArgumentException("Parent set to " + target.ID + ", but that marker has no RemotePosition", "target");
            }

            this.Parent = target;
            target.transform.SetParent(this.transform);
            foreach (Marker marker in this.markerTable.Values)
            {
                Debug.Log("Reparenting" + marker.id + " to:" + this.Parent.id);
                if (marker != this.Parent)
                {
                    marker.transform.SetParent(this.transform);
                    marker.transform.SetParent(target.transform);
                    this.UpdatePosition(marker);
                }
            }
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