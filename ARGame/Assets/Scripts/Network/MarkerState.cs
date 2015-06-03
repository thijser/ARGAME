//----------------------------------------------------------------------------
// <copyright file="MarkerState.cs" company="Delft University of Technology">
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
    /// State of detected marker.
    /// </summary>
    public class MarkerState
    {
        /// <summary>
        /// The factor with which to scale the position.
        /// </summary>
        public const float ScaleFactor = 0.6875f;

        /// <summary>
        /// The vertical offset for the Y coordinate.
        /// </summary>
        public const int VerticalOffset = 72;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerState"/> class.
        /// </summary>
        /// <param name="initialUpdate">The PositionUpdate indicating the initial position.</param>
        /// <param name="referenceMarker">The GameObject that the PositionUpdate represents.</param>
        public MarkerState(PositionUpdate initialUpdate, GameObject referenceMarker)
        {
            if (initialUpdate == null)
            {
                throw new ArgumentNullException("initialUpdate");
            }

            if (referenceMarker == null)
            {
                throw new ArgumentNullException("referenceMarker");
            }

            this.ID = initialUpdate.ID;

            // Create mesh representing this marker
            this.Object = GameObject.Instantiate(referenceMarker);
            this.Object.name = "Marker" + this.ID;

            // Assign number to mesh
            Transform t = this.Object.transform.Find("NumberText");
            if (t == null)
            {
                throw new ArgumentException("The given GameObject does not have a NumberText child object.");
            }

            if (t.GetComponent<TextMesh>() == null)
            {
                throw new ArgumentException("The given GameObject does not have a NumberText child object with a TextMesh component.");
            }

            TextMesh mesh = t.GetComponent<TextMesh>();
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
        /// <param name="positionupdate">The PositionUpdate.</param>
        public void Update(PositionUpdate positionupdate)
        {
            if (positionupdate == null)
            {
                throw new ArgumentNullException("positionupdate");
            }

            if (positionupdate.Type == UpdateType.UpdatePosition)
            {
                // Update orientation of object
                this.Object.SetActive(true);

                this.Object.transform.position = new Vector3(
                    positionupdate.Coordinate.x * ScaleFactor,
                    0,
                    VerticalOffset - (positionupdate.Coordinate.y * ScaleFactor));

                this.Object.transform.eulerAngles = new Vector3(0, positionupdate.Rotation, 0);
            }
            else if (positionupdate.Type == UpdateType.DeletePosition)
            {
                // Remove object
                this.Object.SetActive(false);
            }
        }
    }
}
