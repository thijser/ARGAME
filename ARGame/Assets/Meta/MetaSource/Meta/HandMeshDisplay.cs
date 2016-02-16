using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	internal class HandMeshDisplay : MonoBehaviour, IHandVisualiser<MeshData>
	{
		private MeshData[] _handMeshData;

		private Mesh rightHandMesh;

		private GameObject handright;

		private Mesh leftHandMesh;

		private GameObject handleft;

		[SerializeField]
		private bool _enabled = true;

		private float[] leftHandNewVerticesfloat;

		private Vector3[] leftHandNewVertices;

		private static int left_hand_vertices_length;

		private int[] leftHandNewTriangles;

		private static int left_hand_triangles_length;

		private float[] rightHandNewVerticesfloat;

		private Vector3[] rightHandNewVertices;

		private static int right_hand_vertices_length;

		private int[] rightHandNewTriangles;

		private static int right_hand_triangles_length;

		private float[] leftHandNewSpatialConf;

		private Vector2[] leftHandNewSpatialConfVec2;

		private int[] leftHandNewTemporalConf;

		private Vector2[] leftHandNewTemporalConfVec2;

		private static int left_hand_normals_length;

		private float[] rightHandNewSpatialConf;

		private Vector2[] rightHandNewSpatialConfVec2;

		private int[] rightHandNewTemporalConf;

		private Vector2[] rightHandNewTemporalConfVec2;

		private static int right_hand_normals_length;

		private float[] leftHandNewNormalsfloat;

		private Vector3[] leftHandNewNormals;

		private float[] rightHandNewNormalsfloat;

		private Vector3[] rightHandNewNormals;

		public bool currentlyActive
		{
			get
			{
				return this._enabled;
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "getHandMeshDisplayData")]
		private static extern void GetHandMeshDisplayData(ref MeshData leftHandDisplay, ref MeshData rightHandDisplay);

		[DllImport("MetaVisionDLL", EntryPoint = "enableHandMesh")]
		private static extern void EnableHandMesh();

		[DllImport("MetaVisionDLL", EntryPoint = "disableHandMesh")]
		private static extern void DisableHandMesh();

		private void Start()
		{
			this._handMeshData = new MeshData[2];
			this.rightHandMesh = new Mesh();
			this.leftHandMesh = new Mesh();
			this.handright = GameObject.Find("RightHand");
			this.rightHandMesh = this.handright.GetComponent<MeshFilter>().mesh;
			this.handleft = GameObject.Find("LeftHand");
			this.leftHandMesh = this.handleft.GetComponent<MeshFilter>().mesh;
		}

		private void Update()
		{
			if (this.currentlyActive)
			{
				if (Hands.useFaker)
				{
					MetaOldDLLMetaInputFaker.GetHandMeshDisplayData(ref this._handMeshData[0], ref this._handMeshData[1]);
				}
				else
				{
					this.GetDisplayData(ref this._handMeshData[0], ref this._handMeshData[1]);
				}
				this.rightHandMesh.Clear();
				if (this._handMeshData[1].valid)
				{
					HandMeshDisplay.right_hand_triangles_length = this._handMeshData[1].GetTrianglesLength();
					this.rightHandNewTriangles = new int[HandMeshDisplay.right_hand_triangles_length];
					HandMeshDisplay.right_hand_vertices_length = this._handMeshData[1].GetVerticesLength();
					this.rightHandNewVerticesfloat = new float[HandMeshDisplay.right_hand_vertices_length * 3];
					this.rightHandNewVertices = new Vector3[HandMeshDisplay.right_hand_vertices_length];
					if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
					{
						HandMeshDisplay.right_hand_normals_length = this._handMeshData[1].GetNormalsLength();
						this.rightHandNewNormalsfloat = new float[HandMeshDisplay.right_hand_normals_length * 3];
						this.rightHandNewSpatialConf = new float[HandMeshDisplay.right_hand_vertices_length];
						this.rightHandNewTemporalConf = new int[HandMeshDisplay.right_hand_vertices_length];
						this.rightHandNewNormals = new Vector3[HandMeshDisplay.right_hand_normals_length];
						this.rightHandNewSpatialConfVec2 = new Vector2[HandMeshDisplay.right_hand_vertices_length];
						this.rightHandNewTemporalConfVec2 = new Vector2[HandMeshDisplay.right_hand_vertices_length];
					}
					Marshal.Copy(this._handMeshData[1].triangles, this.rightHandNewTriangles, 0, HandMeshDisplay.right_hand_triangles_length * 1);
					Marshal.Copy(this._handMeshData[1].vertices, this.rightHandNewVerticesfloat, 0, HandMeshDisplay.right_hand_vertices_length * 3);
					if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
					{
						Marshal.Copy(this._handMeshData[1].normals, this.rightHandNewNormalsfloat, 0, HandMeshDisplay.right_hand_normals_length * 3);
						Marshal.Copy(this._handMeshData[1].spatial_conf, this.rightHandNewSpatialConf, 0, HandMeshDisplay.right_hand_vertices_length);
						Marshal.Copy(this._handMeshData[1].temporal_conf, this.rightHandNewTemporalConf, 0, HandMeshDisplay.right_hand_vertices_length);
					}
					for (int i = 0; i < HandMeshDisplay.right_hand_vertices_length; i++)
					{
						this.rightHandNewVertices[i] = new Vector3(this.rightHandNewVerticesfloat[i * 3], this.rightHandNewVerticesfloat[i * 3 + 1], this.rightHandNewVerticesfloat[i * 3 + 2]);
						if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
						{
							this.rightHandNewNormals[i] = new Vector3(this.rightHandNewNormalsfloat[i * 3], this.rightHandNewNormalsfloat[i * 3 + 1], this.rightHandNewNormalsfloat[i * 3 + 2]);
							this.rightHandNewSpatialConfVec2[i] = new Vector2(this.rightHandNewSpatialConf[i] * (float)this.rightHandNewTemporalConf[i], 0f);
							this.rightHandNewTemporalConfVec2[i] = new Vector2((float)this.rightHandNewTemporalConf[i], 0f);
						}
					}
					this.rightHandMesh.vertices = this.rightHandNewVertices;
					if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
					{
						this.rightHandMesh.uv = this.rightHandNewSpatialConfVec2;
						this.rightHandMesh.normals = this.rightHandNewNormals;
					}
					this.rightHandMesh.triangles = this.rightHandNewTriangles;
				}
				this.leftHandMesh.Clear();
				if (this._handMeshData[0].valid)
				{
					HandMeshDisplay.left_hand_triangles_length = this._handMeshData[0].GetTrianglesLength();
					this.leftHandNewTriangles = new int[HandMeshDisplay.left_hand_triangles_length];
					HandMeshDisplay.left_hand_vertices_length = this._handMeshData[0].GetVerticesLength();
					this.leftHandNewVerticesfloat = new float[HandMeshDisplay.left_hand_vertices_length * 3];
					this.leftHandNewVertices = new Vector3[HandMeshDisplay.left_hand_vertices_length];
					if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
					{
						HandMeshDisplay.left_hand_normals_length = this._handMeshData[0].GetNormalsLength();
						this.leftHandNewNormalsfloat = new float[HandMeshDisplay.left_hand_normals_length * 3];
						this.leftHandNewSpatialConf = new float[HandMeshDisplay.left_hand_vertices_length];
						this.leftHandNewTemporalConf = new int[HandMeshDisplay.left_hand_vertices_length];
						this.leftHandNewNormals = new Vector3[HandMeshDisplay.left_hand_normals_length];
						this.leftHandNewSpatialConfVec2 = new Vector2[HandMeshDisplay.left_hand_vertices_length];
						this.leftHandNewTemporalConfVec2 = new Vector2[HandMeshDisplay.left_hand_vertices_length];
					}
					Marshal.Copy(this._handMeshData[0].triangles, this.leftHandNewTriangles, 0, HandMeshDisplay.left_hand_triangles_length * 1);
					Marshal.Copy(this._handMeshData[0].vertices, this.leftHandNewVerticesfloat, 0, HandMeshDisplay.left_hand_vertices_length * 3);
					if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
					{
						Marshal.Copy(this._handMeshData[0].normals, this.leftHandNewNormalsfloat, 0, HandMeshDisplay.left_hand_normals_length * 3);
						Marshal.Copy(this._handMeshData[0].spatial_conf, this.leftHandNewSpatialConf, 0, HandMeshDisplay.left_hand_vertices_length);
						Marshal.Copy(this._handMeshData[0].temporal_conf, this.leftHandNewTemporalConf, 0, HandMeshDisplay.left_hand_vertices_length);
					}
					for (int j = 0; j < HandMeshDisplay.left_hand_vertices_length; j++)
					{
						this.leftHandNewVertices[j] = new Vector3(this.leftHandNewVerticesfloat[j * 3], this.leftHandNewVerticesfloat[j * 3 + 1], this.leftHandNewVerticesfloat[j * 3 + 2]);
						if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
						{
							this.leftHandNewNormals[j] = new Vector3(this.leftHandNewNormalsfloat[j * 3], this.leftHandNewNormalsfloat[j * 3 + 1], this.leftHandNewNormalsfloat[j * 3 + 2]);
							this.leftHandNewSpatialConfVec2[j] = new Vector2(this.leftHandNewSpatialConf[j] * (float)this.leftHandNewTemporalConf[j], 0f);
							this.leftHandNewTemporalConfVec2[j] = new Vector2((float)this.leftHandNewTemporalConf[j], 0f);
						}
					}
					this.leftHandMesh.vertices = this.leftHandNewVertices;
					this.leftHandMesh.triangles = this.leftHandNewTriangles;
					if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
					{
						this.leftHandMesh.uv = this.leftHandNewSpatialConfVec2;
						this.leftHandMesh.normals = this.leftHandNewNormals;
					}
				}
			}
			else
			{
				this.rightHandMesh.Clear();
				this.leftHandMesh.Clear();
			}
		}

		public void OnDestroy()
		{
			this.Disable();
		}

		public void GetDisplayData(ref MeshData leftHandDisplay, ref MeshData rightHandDisplay)
		{
			HandMeshDisplay.GetHandMeshDisplayData(ref leftHandDisplay, ref rightHandDisplay);
		}

		public void Enable()
		{
			this._enabled = true;
		}

		public void Disable()
		{
			this._enabled = false;
		}
	}
}
