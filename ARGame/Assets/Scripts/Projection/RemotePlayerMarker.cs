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
    using System.Collections.Generic;
    using Core.Receiver;

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

        public Material FrustrumMaterial;

        private MeshFilter meshFilter = null;
        private MeshRenderer meshRender = null;

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

                this.UpdateMirrors(transformMatrix);
            }
        }

        public void UpdateMirrors(Matrix4x4 transformMatrix) {
            var markerHolder = GameObject.Find("RemoteController").GetComponent<RemoteMarkerHolder>();

            foreach (var marker in markerHolder.Markers) {
                var mirror = marker.GetComponentInChildren<Mirror>();

                if (mirror != null) {
                    var pos = this.PlayerCamera.WorldToViewportPoint(marker.transform.position);
                    var indicator = marker.transform.Find("PlayerIndicator").GetComponent<MeshRenderer>();

                    if (pos.x >= 0 && pos.y >= 0 && pos.x <= 1 && pos.y <= 1) {
                        Debug.Log("looking at mirror " + marker.Id);
                        indicator.enabled = true;
                    } else {
                        indicator.enabled = false;
                    }
                }
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

            Mesh m = new Mesh();
            m.Clear();

            m.vertices = TransformWorldToLocal((new Vector3[] {
                topLeft + planeNormal,
                topRight + planeNormal,
                bottomRight + planeNormal,
                bottomLeft + planeNormal
            }).ToList());
            m.triangles = new int[] {
                2, 1, 0,
                0, 3, 2
            };

            m.RecalculateBounds();

            meshFilter.mesh = m;
            meshRender.material = FrustrumMaterial;
        }

        private Vector3[] TransformWorldToLocal(List<Vector3> vertices) {
            for (int i = 0; i < vertices.Count; i++) {
                vertices[i] = this.transform.InverseTransformPoint(vertices[i]);
            }
            return vertices.ToArray();
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