//----------------------------------------------------------------------------
// <copyright file="RemoteMarker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using Network;
    using UnityEngine;

    /// <summary>
    /// Represents a marker for a remote player.
    /// </summary>
    public class RemoteMarker : MonoBehaviour
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
        /// Gets the Id of the Marker.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Moves the object to the given coordinates.
        /// <para>
        /// The object is enabled first if it was disabled.
        /// </para>
        /// </summary>
        /// <param name="coordinate">The coordinates to move to.</param>
        public void MoveObject(Vector2 coordinate)
        {
            gameObject.SetActive(true);
            transform.localPosition = new Vector3(
                -(coordinate.x + HorizontalOffset) * ScaleFactor,
                0,
                (coordinate.y + VerticalOffset) * ScaleFactor);
        }

        /// <summary>
        /// Disables the object.
        /// </summary>
        public void RemoveObject()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Changes the rotation of the object to the given rotation.
        /// </summary>
        /// <param name="newRotation">The new rotation.</param>
        public void RotateObject(float newRotation)
        {
            transform.localEulerAngles = new Vector3(0, newRotation, 0);
        }

        /// <summary>
        /// Show the marker Id in the object name.
        /// </summary>
        void Update()
        {
            gameObject.name = "Marker" + this.ID;
        }
    }
}