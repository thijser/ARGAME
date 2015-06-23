//----------------------------------------------------------------------------
// <copyright file="RemoteMarkerHolder.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Previews the position of PositionUpdate objects.
    /// </summary>
    public class RemoteMarkerHolder : MonoBehaviour
    {
        /// <summary>
        /// Time in milliseconds before 
        /// </summary>
        public const long TimeoutTime = 500;

        /// <summary>
        /// Object representing the mesh to use for markers.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public GameObject ReferenceMarker;

        /// <summary>
        /// State of markers in game.
        /// </summary>
        private Dictionary<int, MarkerState> markers = new Dictionary<int, MarkerState>();

        /// <summary>
        /// Receives and handles position updates.
        /// </summary>
        /// <param name="update">The position update to be handled.</param>
        public void OnPositionUpdate(PositionUpdate update)
        {
            RequireMarkerState(update.Id).Update(update);
        }

        /// <summary>
        /// Receives and handles rotation updates.
        /// </summary>
        /// <param name="update">The rotation update to be handled.</param>
        public void OnRotationUpdate(RotationUpdate update)
        {
            RequireMarkerState(update.Id).Update(update);
        }

        /// <summary>
        /// Returns the marker state from the dictionary of markers corresponding with
        /// the given key.
        /// </summary>
        /// <param name="key">The key for searching in the dictionary.</param>
        /// <returns>The marker state corresponding with that key.</returns>
        public MarkerState GetMarkerState(int key)
        {
            return this.markers[key];
        }

        /// <summary>
        /// Get state of marker with given id (will be automatically created if it doesn't exist yet).
        /// </summary>
        /// <param name="id">Id of (new) marker.</param>
        /// <returns>Marker state of marker.</returns>
        public MarkerState RequireMarkerState(int id)
        {
            if (!this.markers.ContainsKey(id))
            {
                this.markers[id] = new MarkerState(id, this.ReferenceMarker);
            }

            return this.markers[id];
        }

        /// <summary>
        /// Disable reference marker, because it's only used as template.
        /// </summary>
        public void Start()
        {
            this.ReferenceMarker.SetActive(false);
        }
    }
}
