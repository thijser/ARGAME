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
    using System.Linq;

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
        /// The <see cref="Material"/> to use for drawing the frustum.
        /// </summary>
        public Material FrustrumMaterial;

        /// <summary>
        /// The <see cref="MeshFilter"/> for the player's frustum.
        /// </summary>
        private MeshFilter meshFilter = null;

        /// <summary>
        /// The <see cref="MeshRenderer"/> for the frustum.
        /// </summary>
        private MeshRenderer meshRender = null;

        /// <summary>
        /// Gets or sets the <see cref="Camera"/> inside this <see cref="RemotePlayerMarker"/>.
        /// </summary>
        public Camera PlayerCamera { get; set; }

        /// <summary>
        /// Gets or sets the color for the player.
        /// </summary>
        public Color PlayerColor
        {
            get { return this.FrustrumMaterial == null ? Color.black : this.FrustrumMaterial.color; }
            set
            {
                if (this.FrustrumMaterial != null)
                {
                    this.FrustrumMaterial = new Material(this.FrustrumMaterial);
                    this.FrustrumMaterial.color = value;
                }
            }
        }

        /// <summary>
        /// Initializes this <see cref="RemotePlayerMarker"/>.
        /// </summary>
        public override void Start()
        {
            base.Start();
            this.PlayerCamera = this.GetComponentInChildren<Camera>();
            this.meshFilter = this.GetComponent<MeshFilter>();
            this.meshRender = this.GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// Updates the position of this <see cref="RemotePlayerMarker"/>.
        /// </summary>
        /// <param name="transformMatrix">The transformation matrix.</param>
        public override void UpdatePosition(Matrix4x4 transformMatrix)
        {
            if (this.RemotePosition != null)
            {
                this.transform.SetFromMatrix(transformMatrix * this.RemotePosition.Matrix);

                this.UpdateFrustum(transformMatrix);
            }
        }

        /// <summary>
        /// Updates the position of the frustum corners on the board.
        /// </summary>
        /// <param name="transformMatrix">The transformation matrix.</param>
        public void UpdateFrustum(Matrix4x4 transformMatrix)
        {
            Rect r = this.PlayerCamera.pixelRect;
            Vector4 topLeft = this.IntersectWithBoard(new Vector2(r.xMin, r.yMin), transformMatrix);
            Vector4 topRight = this.IntersectWithBoard(new Vector2(r.xMax, r.yMin), transformMatrix);
            Vector4 bottomLeft = this.IntersectWithBoard(new Vector2(r.xMin, r.yMax), transformMatrix);
            Vector4 bottomRight = this.IntersectWithBoard(new Vector2(r.xMax, r.yMax), transformMatrix);

            Vector4 planeNormal = transformMatrix * new Vector4(0, 0.1f, 0, 0);

            Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.vertices = (new Vector3[] {
                topLeft + planeNormal,
                topRight + planeNormal,
                bottomRight + planeNormal,
                bottomLeft + planeNormal
            }).Select(vertex => this.transform.InverseTransformPoint(vertex)).ToArray();

            mesh.triangles = new int[] {
                2, 1, 0,
                0, 3, 2
            };

            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;
            meshRender.material = this.FrustrumMaterial;
        }
        
        /// <summary>
        /// Computes the intersection between the board and the ray through
        /// the given screen position of the <see cref="Camera"/> inside this
        /// <see cref="RemotePlayerMarker"/>.
        /// </summary>
        /// <param name="screenPosition">The screen position to raycast through.</param>
        /// <param name="transformMatrix">The transformation matrix.</param>
        /// <returns>The intersection point with the board.</returns>
        private Vector4 IntersectWithBoard(Vector2 screenPosition, Matrix4x4 transformMatrix)
        {
            Ray ray = this.PlayerCamera.ScreenPointToRay(screenPosition);
            Vector4 boardOrigin = transformMatrix * new Vector4(0, 0, 0, 1);
            Vector4 normal = transformMatrix * new Vector4(0, 1, 0, 0);
            Vector4 origin = ray.origin.ToVec4();
            Vector4 direction = ray.direction.ToVec4();

            float t = Vector4.Dot(boardOrigin - origin, normal) / Vector4.Dot(direction, normal);
            Vector4 intersection = origin + t * direction;

            return intersection;
        }
    }
}