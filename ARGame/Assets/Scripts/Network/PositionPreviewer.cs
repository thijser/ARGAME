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
        /// The factor with which to scale the position.
        /// </summary>
        public const float ScaleFactor = 6.9f;

        /// <summary>
        /// The vertical offset for the Y coordinate.
        /// </summary>
        public const int VerticalOffset = 72;

        /// <summary>
        /// Object representing the mesh to use for markers.
        /// </summary>
        public GameObject referenceMarker;

        /// <summary>
        /// State of detected marker.
        /// </summary>
        private class MarkerState
        {
            public int ID;
            public GameObject Object { get; private set; }
            public DateTime LastUpdate { get; private set; }

            public MarkerState(PositionUpdate initialUpdate, GameObject referenceMarker)
            {
                this.ID = initialUpdate.ID;
                this.LastUpdate = new DateTime();

                // Create mesh representing this marker
                this.Object = GameObject.Instantiate(referenceMarker);
                this.Object.name = "Marker" + this.ID;

                // Assign number to mesh
                TextMesh tm = this.Object.transform.Find("NumberText").GetComponent<TextMesh>();
                tm.text = "" + this.ID;
            }

            public void Update(PositionUpdate update)
            {
                this.LastUpdate = DateTime.Now;

                // Update orientation of object
                this.Object.SetActive(true);

                this.Object.transform.position = new Vector3(
                    update.X * ScaleFactor,
                    0,
                    VerticalOffset - (update.Y * ScaleFactor));

                this.Object.transform.eulerAngles = new Vector3(0, update.Rotation, 0);
            }

            public void CheckTimeout()
            {
                if ((DateTime.Now - LastUpdate).TotalMilliseconds > TimeoutTime)
                {
                    this.Object.SetActive(false);
                }
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
                markers[update.ID] = new MarkerState(update, referenceMarker);
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
        /// Disable reference marker, because it's only used as template.
        /// </summary>
        public void Start()
        {
            referenceMarker.SetActive(false);
        }
    }
}
