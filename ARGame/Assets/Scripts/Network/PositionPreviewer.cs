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
        /// Disable reference marker, because it's only used as template.
        /// </summary>
        public void Start()
        {
            this.ReferenceMarker.SetActive(false);
        }

        /// <summary>
        /// State of detected marker.
        /// </summary>
        private class MarkerState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MarkerState"/> class.
            /// </summary>
            /// <param name="initialUpdate">The PositionUpdate indicating the initial position.</param>
            /// <param name="referenceMarker">The GameObject that the PositionUpdate represents.</param>
            public MarkerState(PositionUpdate initialUpdate, GameObject referenceMarker)
            {
                this.ID = initialUpdate.ID;

                // Create mesh representing this marker
                this.Object = GameObject.Instantiate(referenceMarker);
                this.Object.name = "Marker" + this.ID;

                // Assign number to mesh
                TextMesh mesh = this.Object.transform.Find("NumberText").GetComponent<TextMesh>();
                mesh.text = this.ID.ToString();
            }

            /// <summary>
            /// Gets the ID of the Marker.
            /// </summary>
            public int ID { get; private set; }

            /// <summary>
            /// Gets the GameObject assigned to this marker.
            /// </summary>
            public GameObject Object { get; private set; }

            /// <summary>
            /// Updates the position of the GameObject with the given PositionUpdate.
            /// </summary>
            /// <param name="update">The PositionUpdate.</param>
            public void Update(PositionUpdate update)
            {
                if (update.Type == UpdateType.Update)
                {
                    // Update orientation of object
                    this.Object.SetActive(true);

                    this.Object.transform.position = new Vector3(
                        update.X * ScaleFactor,
                        0,
                        VerticalOffset - (update.Y * ScaleFactor));

                    this.Object.transform.eulerAngles = new Vector3(0, update.Rotation, 0);
                }
                else if (update.Type == UpdateType.Delete)
                {
                    // Remove object
                    this.Object.SetActive(false);
                }
            }
        }
    }
}
