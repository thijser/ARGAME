//----------------------------------------------------------------------------
// <copyright file="PositionEntry.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Logging
{
    using System;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Logging entry for marker positions.
    /// </summary>
    public class PositionEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionEntry"/> class 
        /// with default values.
        /// </summary>
        public PositionEntry()
        {
            this.LastMoved = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionEntry"/> class
        /// based on the provided <see cref="PositionUpdate"/>.
        /// </summary>
        /// <param name="update"></param>
        public PositionEntry(PositionUpdate update)
        {
            this.MarkerId = update.Id;
            this.Position = update.Coordinate;
            this.Rotation = update.Rotation;
            this.LastMoved = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the marker id.
        /// </summary>
        public int MarkerId { get; set; }

        /// <summary>
        /// Gets or sets the board position of the marker.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the marker.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the moment when the marker was last moved.
        /// </summary>
        public DateTime LastMoved { get; set; }

        /// <summary>
        /// Formats this <see cref="PositionEntry"/> as a string indicated 
        /// the marker is registered.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string ToCreateString()
        {
            return string.Format(
                "marker #{0} registered, position = ({1}, {2}), rotation = {3}", 
                this.MarkerId, this.Position.x, this.Position.y, this.Rotation);
        }

        /// <summary>
        /// Formats this <see cref="PositionEntry"/> as a string indicating
        /// the marker is moved.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string ToUpdateString()
        {
            return string.Format(
                "marker #{0} moved to position = ({1}, {2}), rotation = {3}",
                this.MarkerId, this.Position.x, this.Position.y, this.Rotation);
        }
    }
}
