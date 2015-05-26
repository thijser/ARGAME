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
    using Assets.Scripts;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Previews the position of PositionUpdate objects.
    /// </summary>
    public class PositionPreviewer : MonoBehaviour
    {
        /// <summary>
        /// Time in milliseconds before 
        /// </summary>
        public const long TimeoutTime = 3000;

        /// <summary>
        /// The factor with which to scale the position.
        /// </summary>
        public const float ScaleFactor = 0.1f;

        /// <summary>
        /// The vertical offset for the Y coordinate.
        /// </summary>
        public const int VerticalOffset = 72;

        /// <summary>
        /// State of detected marker.
        /// </summary>
        private struct MarkerState
        {
            GameObject Object { get; set; }
            long LastUpdate { get; set; }

            public MarkerState(PositionUpdate initialUpdate)
            {
                this.Object = GameObject.Find("Marker" + initialUpdate.ID);
                this.LastUpdate = initialUpdate.TimeStamp;
            }

            public void Update(PositionUpdate update)
            {
                this.LastUpdate = update.TimeStamp;

                // Update orientation of object
                Object.SetEnabled(true);

                Object.transform.position = new Vector3(
                    update.X * ScaleFactor,
                    0,
                    VerticalOffset - (update.Y * ScaleFactor));

                Object.transform.eulerAngles = new Vector3(0, update.Rotation, 0);
            }

            public void CheckTimeout()
            {
                // TODO: Implement
            }
        }

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
            if (!markers.ContainsKey(update.ID))
            {
                markers[update.ID] = new MarkerState(update);
            }

            markers[update.ID].Update(update);
        }

        /// <summary>
        /// Check all markers to see if any need to be hidden because they timed out.
        /// </summary>
        public void Update()
        {
            foreach (var pair in markers)
            {
                pair.Value.CheckTimeout();
            }
        }

        /// <summary>
        /// Disable all markers until they are detected.
        /// </summary>
        public void Start()
        {
            var markerObjects = GameObject.FindGameObjectsWithTag("Marker");

            foreach (var marker in markerObjects)
            {
                marker.SetEnabled(false);
            }
        }
    }
}
