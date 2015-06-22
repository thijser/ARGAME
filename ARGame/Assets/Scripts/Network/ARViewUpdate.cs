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
            Assert.IsTrue(id >= 0);

            this.Type = UpdateType.UpdateARView;
            this.ID = id;
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
    }
}