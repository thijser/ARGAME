//----------------------------------------------------------------------------
// <copyright file="ARViewUpdate.cs" company="Delft University of Technology">
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
    using UnityEngine.Assertions;

    /// <summary>
    /// Update sent by the server when the view of a local player 
    /// changes.
    /// </summary>
    public class ARViewUpdate : AbstractUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ARViewUpdate"/> class.
        /// </summary>
        /// <param name="id">The ID of the player.</param>
        /// <param name="position">The position of the player.</param>
        /// <param name="rotation">The rotation of the player.</param>
        public ARViewUpdate(int id, Vector3 position, Vector3 rotation)
        {
            this.Type = UpdateType.UpdateARView;
            this.Id = id;
            this.Position = position;
            this.Rotation = rotation;
        }

        /// <summary>
        /// Gets the position of the local player.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// Gets the rotation of the local player.
        /// </summary>
        public Vector3 Rotation { get; private set; }

        /// <summary>
        /// Tests whether this <see cref="ARViewUpdate"/> is equal to the 
        /// provided object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns>True if this <see cref="ARViewUpdate"/> is equal to <c>obj</c>, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }
            ARViewUpdate that = obj as ARViewUpdate;
            return this.Id == that.Id
                && this.Position == that.Position
                && this.Rotation == that.Rotation;
        }

        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            int hash = this.GetType().GetHashCode();
            hash = 23 * hash + this.Id;
            hash = 23 * hash + this.Position.GetHashCode();
            hash = 23 * hash + this.Rotation.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a string representation of this <see cref="ARViewUpdate"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="ARViewUpdate"/>.</returns>
        public override string ToString()
        {
            return "<ARViewUpdate[ID=" + this.Id + ", Position=" + this.Position + ", Rotation=" + this.Rotation + "]>";
        }
    }
}