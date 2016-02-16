// Decompiled with JetBrains decompiler
// Type: Meta.HandMeshDisplay
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

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
    private bool _enabled;
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

    public HandMeshDisplay()
    {
      base.\u002Ector();
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
      this.rightHandMesh = ((MeshFilter) this.handright.GetComponent<MeshFilter>()).get_mesh();
      this.handleft = GameObject.Find("LeftHand");
      this.leftHandMesh = ((MeshFilter) this.handleft.GetComponent<MeshFilter>()).get_mesh();
    }

    private void Update()
    {
      if (this.currentlyActive)
      {
        if (Hands.useFaker)
          MetaOldDLLMetaInputFaker.GetHandMeshDisplayData(ref this._handMeshData[0], ref this._handMeshData[1]);
        else
          this.GetDisplayData(ref this._handMeshData[0], ref this._handMeshData[1]);
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
          for (int index = 0; index < HandMeshDisplay.right_hand_vertices_length; ++index)
          {
            this.rightHandNewVertices[index] = new Vector3(this.rightHandNewVerticesfloat[index * 3], this.rightHandNewVerticesfloat[index * 3 + 1], this.rightHandNewVerticesfloat[index * 3 + 2]);
            if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
            {
              this.rightHandNewNormals[index] = new Vector3(this.rightHandNewNormalsfloat[index * 3], this.rightHandNewNormalsfloat[index * 3 + 1], this.rightHandNewNormalsfloat[index * 3 + 2]);
              this.rightHandNewSpatialConfVec2[index] = new Vector2(this.rightHandNewSpatialConf[index] * (float) this.rightHandNewTemporalConf[index], 0.0f);
              this.rightHandNewTemporalConfVec2[index] = new Vector2((float) this.rightHandNewTemporalConf[index], 0.0f);
            }
          }
          this.rightHandMesh.set_vertices(this.rightHandNewVertices);
          if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
          {
            this.rightHandMesh.set_uv(this.rightHandNewSpatialConfVec2);
            this.rightHandMesh.set_normals(this.rightHandNewNormals);
          }
          this.rightHandMesh.set_triangles(this.rightHandNewTriangles);
        }
        this.leftHandMesh.Clear();
        if (!this._handMeshData[0].valid)
          return;
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
        for (int index = 0; index < HandMeshDisplay.left_hand_vertices_length; ++index)
        {
          this.leftHandNewVertices[index] = new Vector3(this.leftHandNewVerticesfloat[index * 3], this.leftHandNewVerticesfloat[index * 3 + 1], this.leftHandNewVerticesfloat[index * 3 + 2]);
          if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
          {
            this.leftHandNewNormals[index] = new Vector3(this.leftHandNewNormalsfloat[index * 3], this.leftHandNewNormalsfloat[index * 3 + 1], this.leftHandNewNormalsfloat[index * 3 + 2]);
            this.leftHandNewSpatialConfVec2[index] = new Vector2(this.leftHandNewSpatialConf[index] * (float) this.leftHandNewTemporalConf[index], 0.0f);
            this.leftHandNewTemporalConfVec2[index] = new Vector2((float) this.leftHandNewTemporalConf[index], 0.0f);
          }
        }
        this.leftHandMesh.set_vertices(this.leftHandNewVertices);
        this.leftHandMesh.set_triangles(this.leftHandNewTriangles);
        if (!MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
          return;
        this.leftHandMesh.set_uv(this.leftHandNewSpatialConfVec2);
        this.leftHandMesh.set_normals(this.leftHandNewNormals);
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
