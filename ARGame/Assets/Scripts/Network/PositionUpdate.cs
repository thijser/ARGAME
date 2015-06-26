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
    using UnityEngine;

    /// <summary>
    /// Represents an serverUpdate of a marker position.
    /// </summary>
    public class PositionUpdate : AbstractUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Network.PositionUpdate"/> class.
        /// </summary>
        /// <param name="type">The type of the PositionUpdate.</param>
        /// <param name="coordinate">A 2D coordinate containing the x- and y-coordinate.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="id">The unique Id of the marker.</param>
        public PositionUpdate(UpdateType type, Vector2 coordinate, float rotation, int id)
        {
            this.Type = type;
            this.Coordinate = coordinate;
            this.Rotation = rotation;
            this.Id = id;
        }

        /// <summary>
        /// Gets the 2D coordinate of this serverUpdate.
        /// </summary>
        public Vector2 Coordinate { get; private set; }

        /// <summary>
        /// Gets the rotation of this serverUpdate.
        /// </summary>
        public float Rotation { get; private set; }

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
            hash = (5 * hash) + this.Id;
            hash = (5 * hash) + this.Coordinate.x.GetHashCode();
            hash = (5 * hash) + this.Coordinate.y.GetHashCode();
            return hash;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to the
        ///   current <see cref="Network.PositionUpdate"/>.
        /// </summary>
        /// <param name="obj">
        ///   The <see cref="System.Object"/> to compare with the current
        ///   <see cref="Network.PositionUpdate"/>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        ///   <see cref="Network.PositionUpdate"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            PositionUpdate that = obj as PositionUpdate;
            return this.Type == that.Type
                && this.Coordinate.x == that.Coordinate.x
                && this.Coordinate.y == that.Coordinate.y
                && this.Id == that.Id;
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents the current <see cref="Network.PositionUpdate"/>.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String"/> that represents the current <see cref="Network.PositionUpdate"/>.
        /// </returns>
        public override string ToString()
        {
            return "[PositionUpdate: Type<" + this.Type + ">, ID<" + this.Id +
                ">, X<" + this.Coordinate.x + ">, Y<" + this.Coordinate.y + ">]";
        }
    }
}
