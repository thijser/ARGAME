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
        }

        private void AddLineSegment(Vector3 from, Vector3 to, List<Vector3> vertices, List<int> triangles)
        {
            int offset = vertices.Count;

            triangles.Add(offset + 0);
            triangles.Add(offset + 1);
            triangles.Add(offset + 2);

            triangles.Add(offset + 2);
            triangles.Add(offset + 3);
            triangles.Add(offset + 0);

            Vector3 left = Vector3.Cross(from - to, Vector3.up).normalized;
            Vector3 up = Vector3.up;

            vertices.Add(from - left * LineWidth / 2);
            vertices.Add(to - left * LineWidth / 2);
            vertices.Add(to + left * LineWidth / 2);
            vertices.Add(from + left * LineWidth / 2);
        }
    }
}