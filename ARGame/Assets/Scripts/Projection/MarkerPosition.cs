//----------------------------------------------------------------------------
// <copyright file="MarkerPosition.cs" company="Delft University of Technology">
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
    using Network;
    using UnityEngine;

    /// <summary>
    /// Keeps data from either remote or local on the position of the marker.
    /// </summary>
    public class MarkerPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerPosition"/> class.
        /// </summary>
        /// <param name="serverUpdate">The serverUpdate, must be of type UpdatePosition.</param>
        public MarkerPosition(PositionUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            if (update.Type != UpdateType.UpdatePosition)
            {
                throw new ArgumentException("UpdateType is not UpdatePosition.", "update");
            }

            this.TimeStamp = DateTime.Now;
            this.Position = new Vector3(8 * update.Coordinate[0], 0, 8 * -update.Coordinate[1]);
            this.Rotation = Quaternion.Euler(0, update.Rotation, 0);
            this.Scale = Vector3.one;
            this.ID = update.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerPosition"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="stamp">The timestamp of the serverUpdate.</param>
        /// <param name="scale">The scale of the object.</param>
        /// <param name="id">The ID of the Marker.</param>
        public MarkerPosition(Vector3 position, Quaternion rotation, DateTime stamp, Vector3 scale, int id)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.TimeStamp = stamp;
            this.Scale = scale;
            this.ID = id;
        }

        /// <summary>
        /// Gets or sets the position of the Marker.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the Marker.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets a timestamp indicating when this MarkerPosition was made.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the scale of the Marker.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Marker.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets a 3D transformation matrix based on the position, rotation 
        /// and scale in this MarkerPosition.
        /// </summary>
        public Matrix4x4 Matrix
        {
            get
            {
                return Matrix4x4.TRS(this.Position, this.Rotation, this.Scale);
            }
        }

        /// <summary>
        /// Returns a string representation of this MarkerPosition.
        /// </summary>
        /// <returns>A string describing this MarkerPosition.</returns>
        public override string ToString()
        {
            return "MarkerPosition:< Position=" + this.Position + ", Rotation=" + this.Rotation + 
                   ", timeStamp=" + this.TimeStamp + ", scale=" + this.Scale + ", id=" + this.ID;
        }
    }
}
