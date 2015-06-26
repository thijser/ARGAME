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
        /// The position of the camera with respect to the rotation point
        /// of the player head.
        /// </summary>
        public static readonly Vector3 CameraOffset = new Vector3(0, 0.10f, 0.15f);

        /// <summary>
        /// Gets the transform Matrix that projects the origin to the camera position.
        /// </summary>
        public Matrix4x4 ZeroToCameraMatrix { get; private set; }

        /// <summary>
        /// Gets the transform Matrix that projects the camera position to the origin.
        /// </summary>
        public Matrix4x4 CameraToZeroMatrix { get; private set; }

        /// <summary>
        /// Initializes the transform matrices for the player camera.
        /// </summary>
        public override void Start()
        {
            base.Start();
            this.ZeroToCameraMatrix = Matrix4x4.TRS(CameraOffset, Quaternion.identity, Vector3.one);
            this.CameraToZeroMatrix = this.ZeroToCameraMatrix.inverse;
        }

        /// <summary>
        /// Updates the position of this <see cref="RemotePlayerMarker"/>.
        /// </summary>
        /// <param name="transformMatrix">The transformation matrix.</param>
        public override void UpdatePosition(Matrix4x4 transformMatrix)
        {
            Matrix4x4 matrix = 
                this.ZeroToCameraMatrix *
                Matrix4x4.TRS(Vector3.zero, this.RemotePosition.Rotation, Vector3.one) *
                this.CameraToZeroMatrix * transformMatrix;
            base.UpdatePosition(matrix);
        }
    }
}