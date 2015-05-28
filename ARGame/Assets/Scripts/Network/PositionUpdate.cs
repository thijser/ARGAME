//----------------------------------------------------------------------------
// <copyright file="PositionUpdate.cs" company="Delft University of Technology">
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

    /// <summary>
    /// Indicates the type of the PositionUpdate message.
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// Indicates the position of the object is updated.
        /// </summary>
        Update = 0,

        /// <summary>
        /// Indicates the object is removed from the field.
        /// </summary>
        Delete = 1
    }

    /// <summary>
    /// Represents an update of a marker position.
    /// </summary>
    public class PositionUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Network.PositionUpdate"/> class.
        /// </summary>
        /// <param name="type">The type of the PositionUpdate.</param> 
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="rotation">The rotation.</param> 
        /// <param name="id">The unique ID of the marker.</param>
        /// <param name="timestamp">The timestamp of the update.</param>
        public PositionUpdate(UpdateType type, float x, float y, float rotation, int id, long timestamp)
        {
            this.Type = type;
            this.X = x;
            this.Y = y;
            this.Rotation = rotation;
            this.ID = id;
            this.TimeStamp = timestamp;
        }

        /// <summary>
        /// Gets the type of this update.
        /// </summary>
        public UpdateType Type { get; private set; }

        /// <summary>
        /// Gets the x coordinate of this update.
        /// </summary>
        public float X { get; private set; }

        /// <summary>
        /// Gets the y coordinate of this update.
        /// </summary>
        public float Y { get; private set; }

        /// <summary>
        /// Gets the rotation of this update.
        /// </summary>
        public float Rotation { get; private set; }

        /// <summary>
        /// Gets the unique ID of the marker object.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the timestamp of this update.
        /// </summary>
        public long TimeStamp { get; private set; }

        /// <summary>
        ///   Serves as a hash function for a <see cref="Network.PositionUpdate"/> object.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance that is suitable for use in hashing 
        ///   algorithms and data structures such as a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = this.Type.GetHashCode();
            hash = (5 * hash) + this.ID;
            hash = (5 * hash) + this.X.GetHashCode();
            hash = (5 * hash) + this.Y.GetHashCode();
            hash = (5 * hash) + this.TimeStamp.GetHashCode();
            return hash;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to the 
        ///   current <see cref="Network.PositionUpdate"/>.
        /// </summary>
        /// <param name="o">
        ///   The <see cref="System.Object"/> to compare with the current 
        ///   <see cref="Network.PositionUpdate"/>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        ///   <see cref="Network.PositionUpdate"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object o)
        {
            if (o == null || o.GetType() != this.GetType())
            {
                return false;
            }

            PositionUpdate that = o as PositionUpdate;
            return this.Type == that.Type
                && this.X == that.X
                && this.Y == that.Y
                && this.ID == that.ID
                && this.TimeStamp == that.TimeStamp;
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents the current <see cref="Network.PositionUpdate"/>.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String"/> that represents the current <see cref="Network.PositionUpdate"/>.
        /// </returns>
        public override string ToString()
        {
            return "[PositionUpdate: Type<" + this.Type + ">, ID<" + this.ID +
                ">, X<" + this.X + ">, Y<" + this.Y + ">, TimeStamp<" + this.TimeStamp + ">]";
        }
    }
}