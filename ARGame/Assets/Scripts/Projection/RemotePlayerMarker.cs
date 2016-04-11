//----------------------------------------------------------------------------
// <copyright file="RemotePlayerMarker.cs" company="Delft University of Technology">
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
    /// Remote marker that represents a player.
    /// <para>
    /// This <see cref="RemoteMarker"/> subclass treats rotations differently, 
    /// because the camera rotation is not determined by the <c>ObjectRotation</c>
    /// field, but directly by the rotation as provided by the <c>RemotePosition</c>.
    /// </para>
    /// </summary>
    public class RemotePlayerMarker : RemoteMarker
    {
        /// <summary>
        /// Updates the position of this <see cref="RemotePlayerMarker"/>.
        /// </summary>
        /// <param name="transformMatrix">The transformation matrix.</param>
        public override void UpdatePosition(Matrix4x4 transformMatrix)
        {
            if (this.RemotePosition != null)
            {
                // The rotation in the RemotePosition is actually an upwards vector here, so we can use
                // Quaternion.LookRotation with the rotation's Euler angles.
                Quaternion rotation = Quaternion.LookRotation(this.RemotePosition.Position, this.RemotePosition.Rotation.eulerAngles);

                Matrix4x4 levelProjection = Matrix4x4.TRS(
                        this.RemotePosition.Position,
                        rotation,
                        this.RemotePosition.Scale);
                this.transform.SetFromMatrix(transformMatrix * levelProjection);
            }
        }
    }
}