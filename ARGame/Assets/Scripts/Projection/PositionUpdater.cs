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
    public class PositionUpdater : MonoBehaviour
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
        private long patience = 1000 * 10000; //// 1000 milliseconds 

        /// <summary>
        /// Gets or sets the central level marker, this should be visible. 
        /// </summary>
        public Marker Parent { get; set; }

        /// <summary>
		/// pre: receive a MarkerRegister object containing a marker.
		/// post: the marker has been registered and can be retrieved by GetMarker
		/// If no parent was active we might as well pick this marker as paretn
        /// Registers a new marker.
        /// </summary>
        /// <param name="register">The marker register parameter that registers the new marker.</param>
        public void OnMarkerRegister(MarkerRegister register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }

            if (this.Parent == null)
            {
                this.Parent = register.RegisteredMarker;
            }
            else
            {
                register.RegisteredMarker.gameObject.transform.SetParent(this.Parent.gameObject.transform);
            }

            this.markerTable.Add(register.RegisteredMarker.ID, register.RegisteredMarker);
        }

        /// <summary>
		/// pre: marker with ID has been registered 
		/// post: returns the marker with id as ID, if none has been found throw exception. 
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
		/// pre: We have a parent with known location 
		/// post: UpdatePosition is called on all markers 
		/// </summary>
        public void Update()
        {
            if (this.Parent == null || this.Parent.LocalPosition == null)
            {
                return;
            }

            foreach (Marker marker in this.markerTable.Values)
            {
                this.UpdatePosition(marker);
            }
        }

        /// <summary>
        /// Receives and handles all server updates.
		/// pre: receive an update from the server 
		/// post: passed on either a position update or a rotationUpdate to the appropriate function 
        /// </summary>
        /// <param name="update">The update to be handled, can be either a
        /// PositionUpdate or a RotationUpdate.</param>
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
        /// This marker has been seen by remote, informs the marker of this 
		/// pre: received a markerPosition from a localy seen marker 
		/// post: The localposition of the marker has been updated, if the parent has not been seen in 
		/// this.patience time then make the new marker into parent and make all other markers children of this new 
		/// parent. Once UpdatePosition has been called on this marker the transform should be updated. 
		/// </summary>
        /// <param name="position">The marker position.</param>
        public void OnMarkerSeen(MarkerPosition position)
        {
            int id = position.ID;
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            this.GetMarker(id).LocalPosition = position;
            if (this.Parent.LocalPosition.TimeStamp.Ticks + this.patience < position.TimeStamp.Ticks)
            {
                this.Reparent(this.GetMarker(id));
            }
        }

        /// <summary>
        /// Inform marker that it has received an rotationUpdate 
		/// pre: received a updationUpdate from another player via the socket 
		/// post: the Object rotation from the marker with the same id as the update is changed, 
		/// once UpdatePosition has been called this should affect the transform of the object. 
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
        /// uses the market target and Parent to set the transform of target
		/// pre: target is a marker is either a parent with a local position or there is a parent with a local position and marker has a remote position. 
		/// post: Transform has been updated. 
		/// </summary>
        /// <param name="target">The target, not null.</param>
        //TODO experimentally determine the specifics. 
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
        /// Updates the Marker position as if the supplied marker 
        /// is a parent marker.
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
        /// Updates position if supplied target is not the Parent.
		/// pre: there is a parent with a local and a remote position. And target is a marker with a remote position 
		/// post: target has a correct yet to be experimentally determined position compared the parent.  
        /// </summary>
        /// <param name="target">The supplied target.</param>
        public void UpdateChildPosition(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (this.Parent.RemotePosition == null)
            {
                throw new NullReferenceException("parent has no remote");
            }

            target.gameObject.transform.position = target.RemotePosition.Position - this.Parent.RemotePosition.Position;
            //// TODO: If mirrored then swap operation params.
        }

        /// <summary>
        /// Changes the parent to the given target Marker.
		/// pre: target is a marker, there are markers in markerTable 
		/// post: target is parent of all other markers and parent is target. 
        /// </summary>
        /// <param name="target">The new parent Marker, not null.</param>
        public void Reparent(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            this.Parent = target;
            foreach (Marker marker in this.markerTable.Values)
            {
                if (marker != this.Parent)
                {
                    marker.transform.SetParent(target.transform);
                    this.UpdatePosition(marker);
                }
            }
        }

        /// <summary>
        /// Updates the location of the marker based on the remote position. 
		/// pre: update.id has been registered, if not registered log a warning, update is a PositionUpdate else throw exception 
		/// post: the remotePosition is updated to be based on the update, see the construction for MarkerPosition(PositionUpdate). 
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