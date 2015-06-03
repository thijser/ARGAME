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
        /// <param name="update">The update, must be of type UpdatePosition.</param>
        public MarkerPosition(PositionUpdate update)
        {
            if (update.Type != UpdateType.UpdatePosition)
            {
                throw new ArgumentException("UpdateType is not UpdatePosition.", "update");
            }

            this.Scale = new Vector3(1, 1, 1);
            this.TimeStamp = DateTime.Now;
            this.Position = new Vector3(update.Coordinate[0], 0, update.Coordinate[1]);
            this.Scale = new Vector3(1, 1, 1);
            this.ID = update.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerPosition"/> class.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="rot">The rotation.</param>
        /// <param name="stamp">The timestamp of the update.</param>
        /// <param name="scale">The scale of the object.</param>
        /// <param name="id">The ID of the Marker.</param>
        public MarkerPosition(Vector3 pos, Quaternion rot, DateTime stamp, Vector3 scale, int id)
        {
            this.Position = pos;
            this.Rotation = rot;
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