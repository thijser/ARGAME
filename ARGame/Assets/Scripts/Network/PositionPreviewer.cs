//----------------------------------------------------------------------------
// <copyright file="PositionPreviewer.cs" company="Delft University of Technology">
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
    public class PositionPreviewer : MonoBehaviour
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
        /// Moves the marker object with the ID of the given PositionUpdate
        /// to the location as indicated by the update.
        /// </summary>
        /// <param name="update">The PositionUpdate object to show, not null.</param>
        public void OnPositionUpdate(PositionUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            // Update marker state (and create initial one if this is the first sighting)
            if (!this.markers.ContainsKey(update.ID))
            {
                this.markers[update.ID] = new MarkerState(update, this.ReferenceMarker);
            }

            this.markers[update.ID].Update(update);
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
        /// Disable reference marker, because it's only used as template.
        /// </summary>
        public void Start()
        {
            this.ReferenceMarker.SetActive(false);
        }
    }
}
