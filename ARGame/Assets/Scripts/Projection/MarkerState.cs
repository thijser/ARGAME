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
    using Projection;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// State of detected marker.
    /// </summary>
    public class MarkerState
    {
        /// <summary>
        /// The factor with which to scale the position.
        /// </summary>
        public const float ScaleFactor = 1f;

        /// <summary>
        /// The horizontal offset for marker positions.
        /// </summary>
        public const float HorizontalOffset = -0.2f;

        /// <summary>
        /// The vertical offset for marker positions.
        /// </summary>
        public const float VerticalOffset = -0.05f;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerState"/> class.
        /// </summary>
        /// <param name="id">The marker Id.</param>
        /// <param name="referenceMarker">The GameObject that the MarkerState represents.</param>
        public MarkerState(int id, GameObject referenceMarker)
        {
            if (referenceMarker == null)
            {
                throw new ArgumentNullException("referenceMarker");
            }

            this.ID = id;

            // Create mesh representing this marker
            this.Object = GameObject.Instantiate(referenceMarker);
            this.Object.name = "Marker" + id;

            // Add reference to this state to game object
            RemoteMarker remoteMarker = this.Object.AddComponent<RemoteMarker>();
            remoteMarker.State = this;
        }

        /// <summary>
        /// Gets the Id of the Marker.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the GameObject assigned to this marker.
        /// </summary>
        public GameObject Object { get; private set; }

        /// <summary>
        /// Moves the object to the given coordinates.
        /// <para>
        /// The object is enabled first if it was disabled.
        /// </para>
        /// </summary>
        /// <param name="coordinate">The coordinates to move to.</param>
        public void MoveObject(Vector2 coordinate)
        {
            this.Object.SetActive(true);
            this.Object.transform.localPosition = new Vector3(
                -(coordinate.x + HorizontalOffset) * ScaleFactor,
                0,
                (coordinate.y + VerticalOffset) * ScaleFactor);
        }

        /// <summary>
        /// Disables the object.
        /// </summary>
        public void RemoveObject()
        {
            this.Object.SetActive(false);
        }

        /// <summary>
        /// Changes the rotation of the object to the given rotation.
        /// </summary>
        /// <param name="newRotation">The new rotation.</param>
        public void RotateObject(float newRotation)
        {
            this.Object.transform.localEulerAngles = new Vector3(0, newRotation, 0);
        }

        /// <summary>
        /// Updates the position/rotation of the GameObject with the given update.
        /// </summary>
        /// <param name="serverUpdate">The update from the server.</param>
        public void Update(AbstractUpdate serverUpdate)
        {
            if (serverUpdate == null)
            {
                throw new ArgumentNullException("serverUpdate");
            }

            Assert.AreEqual(this.ID, serverUpdate.Id, "ID mismatch");
            switch (serverUpdate.Type)
            {
                case UpdateType.UpdatePosition:
                    PositionUpdate positionUpdate = serverUpdate as PositionUpdate;
                    Assert.IsNotNull(positionUpdate);
                    this.MoveObject(positionUpdate.Coordinate);
                    break;
                case UpdateType.DeletePosition:
                    this.RemoveObject();
                    break;
                case UpdateType.UpdateRotation:
                    RotationUpdate rotationUpdate = serverUpdate as RotationUpdate;
                    Assert.IsNotNull(rotationUpdate);
                    this.RotateObject(rotationUpdate.Rotation);
                    break;
                default:
                    break;
            }
        }
    }
}
