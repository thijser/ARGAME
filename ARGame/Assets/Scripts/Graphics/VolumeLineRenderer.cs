//----------------------------------------------------------------------------
// <copyright file="VolumeLineRenderer.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;
    using UnityEngine.Rendering;

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
        /// Material to apply to line mesh.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Material LineMaterial;

        /// <summary>
        /// Width and height of line mesh.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public float LineWidth = 1.0f;

        /// <summary>
        /// Indicates if specified positions lie in world space.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public bool UseWorldSpace = false;

        /// <summary>
        /// Indicates if generated line mesh should cast shadows.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public bool CastShadows = true;

        /// <summary>
        /// Indicates if generated line mesh should receive shadows.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
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
        /// The corner points of the line segments in this VolumeLineRenderer.
        /// </summary>
        private Vector3[] positions = new Vector3[0];

        /// <summary>
        /// Gets a Read-Only Collection of all positions in this VolumeLineRenderer.
        /// </summary>
        public ReadOnlyCollection<Vector3> Positions
        {
            get
            {
                return Array.AsReadOnly(this.positions);
            }
        }

        /// <summary>
        /// Initializes this VolumeLineRenderer.
        /// </summary>
        public void Start()
        {
            // Create components for rendering line mesh
            this.meshFilter = gameObject.AddComponent<MeshFilter>();
            this.meshRenderer = gameObject.AddComponent<MeshRenderer>();

            this.meshFilter.mesh = new Mesh();
        }

        /// <summary>
        /// Updates the line Mesh and renders it.
        /// </summary>
        public void Update()
        {
            // Update line material
            this.meshRenderer.material = this.LineMaterial;

            // Create mesh that represents line
            Mesh mesh = this.meshFilter.mesh;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            this.CreateLineMesh(vertices, triangles);

            // Transform mesh vertices if using world space
            if (this.UseWorldSpace)
            {
                this.TransformWorldToLocal(vertices);
            }

            // Update mesh and render bounds
            mesh.triangles = new int[0];

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            this.meshRenderer.shadowCastingMode = this.CastShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            this.meshRenderer.receiveShadows = this.ReceiveShadows;
        }

        /// <summary>
        /// Change the amount of positions in the line.
        /// </summary>
        /// <param name="count">Amount of positions in line.</param>
        public void SetVertexCount(int count)
        {
            if (this.Positions.Count != count)
            {
                Vector3[] newPositions = new Vector3[count];

                // Copy positions from original array if available,
                // otherwise initialize to zero.
                for (int i = 0; i < count; i++)
                {
                    if (i < this.positions.Length)
                    {
                        newPositions[i] = this.positions[i];
                    }
                    else
                    {
                        newPositions[i] = Vector3.zero;
                    }
                }

                this.positions = newPositions;
            }
        }

        /// <summary>
        /// Set a certain position in the line sequence.
        /// </summary>
        /// <param name="index">Position index, between 0 and <c>Positions.Count</c>.</param>
        /// <param name="position">New position value.</param>
        public void SetPosition(int index, Vector3 position)
        {
            if (index < 0 || index >= this.Positions.Count)
            {
                throw new ArgumentOutOfRangeException("index", index, "index should be between 0 and " + this.Positions.Count);
            }

            this.positions[index] = position;
        }

        /// <summary>
        /// Generate triangles for cube between two quads.
        /// </summary>
        /// <param name="from">Points of start quad.</param>
        /// <param name="to">Points of end quad.</param>
        /// <returns>List of triangle vertices.</returns>
        private static List<Vector3> CubeVerticesBetween(Vector3[] from, Vector3[] to)
        {
            List<Vector3> vertices = new List<Vector3>(24);

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

        /// <summary>
        /// Create the line mesh and add its vertices and triangles to the lists.
        /// </summary>
        /// <param name="vertices">List to append vertices to.</param>
        /// <param name="triangles">List to append triangles to.</param>
        private void CreateLineMesh(List<Vector3> vertices, List<int> triangles)
        {
            for (int i = 0; i < this.positions.Length - 1; i++)
            {
                // Add a cube segment between two positions
                this.AddLineSegment(this.positions[i], this.positions[i + 1], vertices, triangles);

                // Add a connecting segment inbetween
                if (i > 0)
                {
                    this.AddConnectingSegment(
                        new Vector3[] 
                        {
                            this.positions[i - 1],
                            this.positions[i],
                            this.positions[i + 1],
                        }, 
                        vertices, 
                        triangles);
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
                vertices[i] = this.transform.InverseTransformPoint(vertices[i]);
            }
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
            Vector3 up = this.transform.up;
            Vector3 left = Vector3.Cross(from - to, up).normalized;

            float halfLineWidth = this.LineWidth / 2.0f;

            // Determine endpoints of segment cube
            Vector3[] fromPoints = new Vector3[] 
            {
                from - (left * halfLineWidth) - (up * halfLineWidth),
                from - (left * halfLineWidth) + (up * halfLineWidth),
                from + (left * halfLineWidth) - (up * halfLineWidth),
                from + (left * halfLineWidth) + (up * halfLineWidth)
            };

            Vector3[] toPoints = new Vector3[] 
            {
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

            float halfLineWidth = this.LineWidth / 2.0f;

            // Determine endpoints of cube connecting segments
            Vector3[] fromPoints = new Vector3[] 
            {
                between[1] - (leftFrom * halfLineWidth) - (up * halfLineWidth),
                between[1] - (leftFrom * halfLineWidth) + (up * halfLineWidth),
                between[1] + (leftFrom * halfLineWidth) - (up * halfLineWidth),
                between[1] + (leftFrom * halfLineWidth) + (up * halfLineWidth)
            };

            Vector3[] toPoints = new Vector3[] 
            {
                between[1] - (leftTo * halfLineWidth) - (up * halfLineWidth),
                between[1] - (leftTo * halfLineWidth) + (up * halfLineWidth),
                between[1] + (leftTo * halfLineWidth) - (up * halfLineWidth),
                between[1] + (leftTo * halfLineWidth) + (up * halfLineWidth)
            };

            List<Vector3> cubeVertices = CubeVerticesBetween(fromPoints, toPoints);

            // Add generated cube to mesh
            for (int i = 0; i < cubeVertices.Count; i++)
            {
                triangles.Add(vertices.Count + i);
            }

            vertices.AddRange(cubeVertices);
        }
    }
}