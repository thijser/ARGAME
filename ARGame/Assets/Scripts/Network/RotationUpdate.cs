//----------------------------------------------------------------------------
// <copyright file="RotationUpdate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    /// <summary>
    /// An serverUpdate object that describes a changed rotation of a game object.
    /// The rotation is caused by the remote player.
    /// </summary>
    public class RotationUpdate : AbstractUpdate 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Network.RotationUpdate"/> class.
        /// </summary>
        /// <param name="type">The serverUpdate type.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="id">The ID of this serverUpdate.</param>
        public RotationUpdate(UpdateType type, float rotation, int id)
        {
            this.Type = type;
            this.Rotation = rotation;
            this.ID = id;
        }

        /// <summary>
        /// Gets the rotation if this serverUpdate object.
        /// </summary>
        public float Rotation { get; private set; }

        /// <summary>
        /// Tests whether this RotationUpdate is equal to the given object
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns>True if this RotationObject is equal to <c>obj</c>, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            RotationUpdate that = obj as RotationUpdate;
            if (that == null)
            {
                return false;
            }

            return this.ID == that.ID && this.Rotation == that.Rotation;
        }

        /// <summary>
        /// Returns a hash code for this RotationUpdate.
        /// </summary>
        /// <returns>a hash code.</returns>
        public override int GetHashCode()
        {
            return (typeof(RotationUpdate).GetHashCode() * this.ID) ^ this.Rotation.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this RotationUpdate.
        /// </summary>
        /// <returns>A string describing this RotationUpdate.</returns>
        public override string ToString()
        {
            return "<RotationUpdate[ID=" + this.ID + ", Rotation=" + this.Rotation + "]>";
        }
    }
}
