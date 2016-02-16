// Decompiled with JetBrains decompiler
// Type: Meta.HandPointCloudDisplay
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  internal class HandPointCloudDisplay : MonoBehaviour, IHandVisualiser<PointCloudData>
  {
    private ParticleSystem.Particle[] m_cloud;
    private PointCloudData[] _handPointCloud;
    private float[][] _handPointCloudVertices;
    [SerializeField]
    private Color _particleColor;
    [SerializeField]
    [Range(0.001f, 1f)]
    private float _particleSize;
    private bool _enabled;

    public bool currentlyActive
    {
      get
      {
        return this._enabled;
      }
    }

    public HandPointCloudDisplay()
    {
      base.\u002Ector();
    }

    [DllImport("MetaVisionDLL", EntryPoint = "getPointCloudDisplayData")]
    private static extern void GetPointCloudDisplayData(ref PointCloudData leftHandDisplay, ref PointCloudData rightHandDisplay);

    [DllImport("MetaVisionDLL", EntryPoint = "enableHandPointCloud")]
    private static extern void EnableHandPointCloud();

    [DllImport("MetaVisionDLL", EntryPoint = "disableHandPointCloud")]
    private static extern void DisableHandPointCloud();

    private void Start()
    {
      this.m_cloud = new ParticleSystem.Particle[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices];
      for (int index = 0; index < 2; ++index)
      {
        this._handPointCloud[index] = new PointCloudData();
        this._handPointCloud[index].Init();
        this._handPointCloudVertices[index] = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
      }
      if (Hands.useFaker || !this._enabled)
        return;
      this.Enable();
    }

    private void Update()
    {
      if (!this.currentlyActive)
        return;
      this.GetDisplayData(ref this._handPointCloud[0], ref this._handPointCloud[1]);
      int offset = 0;
      for (int index = 0; index < 2; ++index)
      {
        if (this._handPointCloud[index].vertices != IntPtr.Zero)
          Marshal.Copy(this._handPointCloud[index].vertices, this._handPointCloudVertices[index], 0, this._handPointCloud[index].size * 3);
        this.SetParticlePoints(this._handPointCloudVertices[index], this._handPointCloud[index].size, offset);
        offset += this._handPointCloud[index].size;
      }
      ((ParticleSystem) ((Component) this).GetComponent<ParticleSystem>()).SetParticles(this.m_cloud, offset);
    }

    private void OnDestroy()
    {
      if (Hands.useFaker)
        return;
      this.Disable();
    }

    public void GetDisplayData(ref PointCloudData leftHandDisplay, ref PointCloudData rightHandDisplay)
    {
      if (Hands.useFaker)
        MetaOldDLLMetaInputFaker.GetPointCloudDisplayData(ref leftHandDisplay, ref rightHandDisplay);
      else
        HandPointCloudDisplay.GetPointCloudDisplayData(ref leftHandDisplay, ref rightHandDisplay);
    }

    public void Enable()
    {
      HandPointCloudDisplay.EnableHandPointCloud();
      this._enabled = true;
    }

    public void Disable()
    {
      HandPointCloudDisplay.DisableHandPointCloud();
      this._enabled = false;
    }

    private void SetParticlePoints(float[] points, int size, int offset)
    {
      for (int index = 0; index < size; ++index)
      {
        // ISSUE: explicit reference operation
        ((ParticleSystem.Particle) @this.m_cloud[index + offset]).set_position(new Vector3(points[3 * index], points[3 * index + 1], points[3 * index + 2] - (float) ((Component) this).get_transform().get_localPosition().z));
        // ISSUE: explicit reference operation
        ((ParticleSystem.Particle) @this.m_cloud[index + offset]).set_color(Color32.op_Implicit(this._particleColor));
        // ISSUE: explicit reference operation
        ((ParticleSystem.Particle) @this.m_cloud[index + offset]).set_size(this._particleSize);
      }
    }
  }
}
