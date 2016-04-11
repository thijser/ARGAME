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
        /// The <see cref="Camera"/> inside this <see cref="RemotePlayerMarker"/>.
        /// </summary>
        public Camera PlayerCamera { get; set; }

        /// <summary>
        /// Initializes this <see cref="RemotePlayerMarker"/>.
        /// </summary>
        public override void Start()
        {
            base.Start();
            this.PlayerCamera = this.GetComponentInChildren<Camera>();
        }

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

                this.UpdateFrustum(transformMatrix);
            }
        }

        /// <summary>
        /// Updates the position of the frustum corners on the board.
        /// </summary>
        public void UpdateFrustum(Matrix4x4 transformMatrix)
        {
            Rect r = this.PlayerCamera.pixelRect;
            Vector4 topLeft = this.IntersectWithBoard(new Vector2(r.xMin, r.yMin), transformMatrix);
            Vector4 topRight = this.IntersectWithBoard(new Vector2(r.xMax, r.yMin), transformMatrix);
            Vector4 bottomLeft = this.IntersectWithBoard(new Vector2(r.xMin, r.yMax), transformMatrix);
            Vector4 bottomRight = this.IntersectWithBoard(new Vector2(r.xMax, r.yMax), transformMatrix);

            GameObject.Find("TopLeft").transform.position = topLeft;
            GameObject.Find("TopRight").transform.position = topRight;
            GameObject.Find("BottomLeft").transform.position = bottomLeft;
            GameObject.Find("BottomRight").transform.position = bottomRight;
        }

        /// <summary>
        /// Computes the intersection between the board and the ray through
        /// the given screen position of the <see cref="Camera"/> inside this
        /// <see cref="RemotePlayerMarker"/>.
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        private Vector4 IntersectWithBoard(Vector2 screenPosition, Matrix4x4 transformMatrix)
        {
            Ray ray = this.PlayerCamera.ScreenPointToRay(screenPosition);
            Vector4 normal = transformMatrix * new Vector4(0, 1, 0, 1);
            Vector4 origin = ray.origin.ToVec4();
            Vector4 direction = ray.direction.ToVec4();

            float t = -Vector4.Dot(origin, normal) / Vector4.Dot(direction, normal);
            Vector4 intersection = origin + t * direction;

            return intersection;
        }
    }
}