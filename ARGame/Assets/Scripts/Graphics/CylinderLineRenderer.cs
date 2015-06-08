using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Graphics
{
    /// <summary>
    /// Custom LineRenderer for lasers that doesn't have the rendering problems
    /// that occur in the Unity LineRenderer implementation.
    /// 
    /// NOTE: Implementation assumes that up is always [0, 1, 0].
    /// </summary>
    public class CylinderLineRenderer : MonoBehaviour
    {
        public Vector3[] Positions;
        public Material LineMaterial;
        public float LineWidth = 1.0f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public void Start()
        {
            // Create components for rendering line mesh
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = new Mesh();
        }

        public void Update()
        {
            // Update line material
            meshRenderer.material = LineMaterial;

            // Create mesh that represents line
            Mesh mesh = meshFilter.mesh;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < Positions.Length - 1; i++)
            {
                AddLineSegment(Positions[i], Positions[i + 1], vertices, triangles);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private void AddLineSegment(Vector3 from, Vector3 to, List<Vector3> vertices, List<int> triangles)
        {
            int offset = vertices.Count;

            // Bottom
            triangles.Add(offset + 4);
            triangles.Add(offset + 2);
            triangles.Add(offset + 0);

            triangles.Add(offset + 0);
            triangles.Add(offset + 6);
            triangles.Add(offset + 4);

            // Top
            triangles.Add(offset + 1);
            triangles.Add(offset + 3);
            triangles.Add(offset + 5);

            triangles.Add(offset + 5);
            triangles.Add(offset + 7);
            triangles.Add(offset + 1);

            // Left
            triangles.Add(offset + 0);
            triangles.Add(offset + 2);
            triangles.Add(offset + 3);

            triangles.Add(offset + 3);
            triangles.Add(offset + 1);
            triangles.Add(offset + 0);

            // Right
            triangles.Add(offset + 4);
            triangles.Add(offset + 6);
            triangles.Add(offset + 7);

            triangles.Add(offset + 7);
            triangles.Add(offset + 5);
            triangles.Add(offset + 4);

            // First endpoint
            triangles.Add(offset + 0);
            triangles.Add(offset + 1);
            triangles.Add(offset + 7);

            triangles.Add(offset + 7);
            triangles.Add(offset + 6);
            triangles.Add(offset + 0);

            // Second endpoint
            triangles.Add(offset + 5);
            triangles.Add(offset + 3);
            triangles.Add(offset + 2);

            triangles.Add(offset + 2);
            triangles.Add(offset + 4);
            triangles.Add(offset + 5);

            Vector3 left = Vector3.Cross(from - to, Vector3.up).normalized;
            Vector3 up = Vector3.up;

            float halfLineWidth = LineWidth / 2.0f;

            vertices.Add(from - left * halfLineWidth - up * halfLineWidth); // 0
            vertices.Add(from - left * halfLineWidth + up * halfLineWidth); // 1

            vertices.Add(to - left * halfLineWidth - up * halfLineWidth); // 2
            vertices.Add(to - left * halfLineWidth + up * halfLineWidth); // 3

            vertices.Add(to + left * halfLineWidth - up * halfLineWidth); // 4
            vertices.Add(to + left * halfLineWidth + up * halfLineWidth); // 5

            vertices.Add(from + left * halfLineWidth - up * halfLineWidth); // 6
            vertices.Add(from + left * halfLineWidth + up * halfLineWidth); // 7
        }
    }
}