//----------------------------------------------------------------------------
// <copyright file="Marker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using UnityEngine;

    /// <summary>
    /// Represents a marker in the world.
    /// </summary>
    public class Marker : MonoBehaviour
    {
        /// <summary>
        /// The id of the marker.
        /// </summary>
        private int id = -1;

        /// <summary>
        /// Gets or sets the id of this Marker.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                gameObject.name = "Marker_" + value;
            }
        }

        /// <summary>
        /// Gets or sets the position of this Marker as seen from the remote server.
        /// </summary>
        public MarkerPosition RemotePosition { get; set; }

        /// <summary>
        /// Gets or sets the actual rotation of this UnityEngine.Object.
        /// </summary>
        public float ObjectRotation { get; set; }

        /// <summary>
        /// Updates the position of this marker using the provided remote-to-camera
        /// transformation matrix.
        /// </summary>
        /// <param name="transformMatrix">The transformation matrix to use.</param>
        public virtual void UpdatePosition(Matrix4x4 transformMatrix)
        {
            if (this.RemotePosition != null)
            {
                Matrix4x4 levelProjection = Matrix4x4.TRS(
                        this.RemotePosition.Position,
                        Quaternion.Euler(0, this.ObjectRotation, 0),
                        this.RemotePosition.Scale);
                this.transform.SetFromMatrix(transformMatrix * levelProjection);
            }
        }
    }
}
