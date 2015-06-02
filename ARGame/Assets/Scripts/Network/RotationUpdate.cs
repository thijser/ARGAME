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
    /// An update object that describes a changed rotation of a game object.
    /// The rotation is caused by the remote player.
    /// </summary>
    public class RotationUpdate : AbstractUpdate 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Network.RotationUpdate"/> class.
        /// </summary>
        /// <param name="type">The update type.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="id">The ID of this update.</param>
        public RotationUpdate(UpdateType type, float rotation, int id)
        {
            this.Type = type;
            this.Rotation = rotation;
            this.ID = id;
        }

        /// <summary>
        /// Gets the rotation if this update object.
        /// </summary>
        public float Rotation { get; private set; }
    }
}
