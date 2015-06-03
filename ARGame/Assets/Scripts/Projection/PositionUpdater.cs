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
        private float scale = 1;

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
                this.UpdatePosition(marker);
            }
        }

        /// <summary>
        /// This marker has been seen by remote, informs the marker of this 
        /// </summary>
        /// <param name="position">The marker position.</param>
        /// <param name="id">The identifier.</param>
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
        /// </summary>
        /// <param name="update">rotation update.</param>
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
        /// </summary>
        /// <param name="target">The target</param>
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
        /// Updates position if supplied target is the Parent.
        /// </summary>
        /// <param name="target">The supplied target.</param>
        public void UpdateParentPosition(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
			if(target.LocalPosition== null){
				throw new ArgumentNullException("parent has no localposition");
			}                        
			Debug.Log (target);
            target.gameObject.transform.position = target.LocalPosition.Position;
            Vector3 localrotation = target.LocalPosition.Rotation.eulerAngles;
            Vector3 remoterotation = target.RemotePosition.Rotation.eulerAngles;
            Vector3 finalrotation = localrotation - remoterotation;
            target.gameObject.transform.rotation = Quaternion.Euler(finalrotation);
        }

        /// <summary>
        /// Updates position if supplied target is not the Parent.
        /// </summary>
        /// <param name="target">The supplied target.</param>
        public void UpdateChildPosition(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
			if(this.Parent.RemotePosition==null){
				throw new NullReferenceException("parent has no remote");
			}

            target.gameObject.transform.position = target.RemotePosition.Position - this.Parent.RemotePosition.Position;
            //// TODO: If mirrored then swap operation params.
        }

        /// <summary>
        /// Changes the parent to the given target Marker.
        /// </summary>
        /// <param name="target">The new parent Marker, not null.</param>
        public void Reparent(Marker target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            this.Parent = target;
            foreach (KeyValuePair<int, Marker> entry in this.markerTable)
            {
                if (entry.Value != this.Parent)
                {
                    entry.Value.transform.SetParent(target.transform);
                    this.UpdatePosition(entry.Value);
                }
            }
        }

        /// <summary>
        /// Updates the location of the marker based on the remote position. 
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