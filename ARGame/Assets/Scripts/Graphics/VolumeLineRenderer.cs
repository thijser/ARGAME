using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace Graphics
{
    /// <summary>
    /// Custom LineRenderer for lasers that doesn't have the rendering problems
    /// that occur in the Unity LineRenderer implementation.
    /// <para>
    /// This implementation assumes that lines lie in the plane of the emitter.
    /// </para>
    /// </summary>
    public class VolumeLineRenderer : MonoBehaviour
    {
        /// <summary>
        /// Positions that line passes through.
        /// </summary>
        public Vector3[] Positions = new Vector3[0];

        /// <summary>
        /// Material to apply to line mesh.
        /// </summary>
        public Material LineMaterial;

        /// <summary>
        /// Width and height of line mesh.
        /// </summary>
        public float LineWidth = 1.0f;

        /// <summary>
        /// Indicates if specified positions lie in world space.
        /// </summary>
        public bool UseWorldSpace = false;

        /// <summary>
        /// Indicates if generated line mesh should cast shadows.
        /// </summary>
        public bool CastShadows = true;

        /// <summary>
        /// Indicates if generated line mesh should receive shadows.
        /// </summary>
        public bool ReceiveShadows = true;

        /// <summary>
        /// The MeshFilter Component that contains the Mesh.
        /// </summary>
        private MeshFilter meshFilter;

        /// <summary>
        /// The MeshRenderer Component that renders the Mesh.
        /// </summary>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// Initializes this VolumeLineRenderer.
        /// </summary>
        public void Start()
        {
            // Create components for rendering line mesh
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = new Mesh();
        }

        /// <summary>
        /// Updates the line Mesh and renders it.
        /// </summary>
        public void Update()
        {
            // Update line material
            meshRenderer.material = LineMaterial;

            // Create mesh that represents line
            Mesh mesh = meshFilter.mesh;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            CreateLineMesh(vertices, triangles);

            // Transform mesh vertices if using world space
            if (UseWorldSpace)
            {
                TransformWorldToLocal(vertices);
            }

            // Update mesh and render bounds
            mesh.triangles = new int[0];

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshRenderer.shadowCastingMode = CastShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            meshRenderer.receiveShadows = ReceiveShadows;
        }

        /// <summary>
        /// Create the line mesh and add its vertices and triangles to the lists.
        /// </summary>
        /// <param name="vertices">List to append vertices to.</param>
        /// <param name="triangles">List to append triangles to.</param>
        private void CreateLineMesh(List<Vector3> vertices, List<int> triangles)
        {
            for (int i = 0; i < Positions.Length - 1; i++)
            {
                // Add a cube segment between two positions
                AddLineSegment(Positions[i], Positions[i + 1], vertices, triangles);

                // Add a connecting segment inbetween
                if (i > 0)
                {
                    AddConnectingSegment(new Vector3[] {
                        Positions[i - 1],
                        Positions[i],
                        Positions[i + 1],
                    }, vertices, triangles);
                }
            }
        }

        /// <summary>
        /// Transform vertices from world coordinates to local coordinates.
        /// </summary>
        /// <param name="vertices">Vertices to transform.</param>
        private void TransformWorldToLocal(List<Vector3> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 worldPos = Quaternion.Inverse(transform.rotation) * (vertices[i] - transform.position);
                vertices[i] = new Vector3(
                    worldPos.x / transform.localScale.x,
                    worldPos.y / transform.localScale.y,
                    worldPos.z / transform.localScale.z
                );
            }
        }

        /// <summary>
        /// Change the amount of positions in the line.
        /// </summary>
        /// <param name="count">Amount of positions in line.</param>
        public void SetVertexCount(int count)
        {
            if (this.Positions.Length != count)
            {
                Vector3[] newPositions = new Vector3[count];

                // Copy positions from original array if available,
                // otherwise initialize to zero.
                for (int i = 0; i < count; i++)
                {
                    if (i < this.Positions.Length)
                    {
                        newPositions[i] = this.Positions[i];
                    }
                    else
                    {
                        newPositions[i] = Vector3.zero;
                    }
                }

                this.Positions = newPositions;
            }
        }

        /// <summary>
        /// Set a certain position in the line sequence.
        /// </summary>
        /// <param name="index">Position index.</param>
        /// <param name="pos">New position value.</param>
        public void SetPosition(int index, Vector3 pos)
        {
            this.Positions[index] = pos;
        }

        /// <summary>
        /// Adds a cube between the two positions alongside the plane of the emitter.
        /// </summary>
        /// <param name="from">Start position of cube.</param>
        /// <param name="to">End position of cube.</param>
        /// <param name="vertices">Vertex list to append to.</param>
        /// <param name="triangles">Triangle list to append to.</param>
        private void AddLineSegment(Vector3 from, Vector3 to, List<Vector3> vertices, List<int> triangles)
        {
            // Determine endpoints of segment cube
            Vector3 up = transform.up;
            Vector3 left = Vector3.Cross(from - to, up).normalized;

            float halfLineWidth = LineWidth / 2.0f;

            // Determine endpoints of segment cube
            Vector3[] fromPoints = new Vector3[] {
                from - left * halfLineWidth - up * halfLineWidth,
                from - left * halfLineWidth + up * halfLineWidth,
                from + left * halfLineWidth - up * halfLineWidth,
                from + left * halfLineWidth + up * halfLineWidth
            };

            Vector3[] toPoints = new Vector3[] {
                fromPoints[0] - from + to,
                fromPoints[1] - from + to,
                fromPoints[2] - from + to,
                fromPoints[3] - from + to
            };

            List<Vector3> cubeVertices = CubeVerticesBetween(fromPoints, toPoints);

            // Add generated cube to mesh
            for (int i = 0; i < cubeVertices.Count; i++)
            {
                triangles.Add(vertices.Count + i);
            }

            vertices.AddRange(cubeVertices);
        }

        /// <summary>
        /// Adds a cube between two segments to close the corner.
        /// </summary>
        /// <param name="between">Three points: from, center and to.</param>
        /// <param name="vertices">Vertex list to append to.</param>
        /// <param name="triangles">Triangle list to append to.</param>
        private void AddConnectingSegment(Vector3[] between, List<Vector3> vertices, List<int> triangles)
        {
            // Determine endpoints of two segments
            Vector3 up = transform.up;
            Vector3 leftFrom = Vector3.Cross(between[1] - between[0], up).normalized;
            Vector3 leftTo = Vector3.Cross(between[2] - between[1], up).normalized;

            float halfLineWidth = LineWidth / 2.0f;

            // Determine endpoints of cube connecting segments
            Vector3[] fromPoints = new Vector3[] {
                between[1] - leftFrom * halfLineWidth - up * halfLineWidth,
                between[1] - leftFrom * halfLineWidth + up * halfLineWidth,
                between[1] + leftFrom * halfLineWidth - up * halfLineWidth,
                between[1] + leftFrom * halfLineWidth + up * halfLineWidth
            };

            Vector3[] toPoints = new Vector3[] {
                between[1] - leftTo * halfLineWidth - up * halfLineWidth,
                between[1] - leftTo * halfLineWidth + up * halfLineWidth,
                between[1] + leftTo * halfLineWidth - up * halfLineWidth,
                between[1] + leftTo * halfLineWidth + up * halfLineWidth
            };

            List<Vector3> cubeVertices = CubeVerticesBetween(fromPoints, toPoints);

            // Add generated cube to mesh
            for (int i = 0; i < cubeVertices.Count; i++)
            {
                triangles.Add(vertices.Count + i);
            }

            vertices.AddRange(cubeVertices);
        }

        /// <summary>
        /// Generate triangles for cube between two quads.
        /// </summary>
        /// <param name="from">Points of start quad.</param>
        /// <param name="to">Points of end quad.</param>
        /// <returns>List of triangle vertices.</returns>
        private static List<Vector3> CubeVerticesBetween(Vector3[] from, Vector3[] to)
        {
            List<Vector3> vertices = new List<Vector3>();

            // Bottom
            vertices.Add(to[2]);
            vertices.Add(to[0]);
            vertices.Add(from[0]);

            vertices.Add(from[0]);
            vertices.Add(from[2]);
            vertices.Add(to[2]);

            // Top
            vertices.Add(from[1]);
            vertices.Add(to[1]);
            vertices.Add(to[3]);

            vertices.Add(to[3]);
            vertices.Add(from[3]);
            vertices.Add(from[1]);

            // Left
            vertices.Add(from[0]);
            vertices.Add(to[0]);
            vertices.Add(to[1]);

            vertices.Add(to[1]);
            vertices.Add(from[1]);
            vertices.Add(from[0]);

            // Right
            vertices.Add(to[2]);
            vertices.Add(from[2]);
            vertices.Add(from[3]);

            vertices.Add(from[3]);
            vertices.Add(to[3]);
            vertices.Add(to[2]);

            // Source endpoint
            vertices.Add(from[0]);
            vertices.Add(from[1]);
            vertices.Add(from[3]);

            vertices.Add(from[3]);
            vertices.Add(from[2]);
            vertices.Add(from[0]);

            // Destination endpoint
            vertices.Add(to[3]);
            vertices.Add(to[1]);
            vertices.Add(to[0]);

            vertices.Add(to[0]);
            vertices.Add(to[2]);
            vertices.Add(to[3]);

            return vertices;
        }
    }
}