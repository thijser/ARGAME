// Decompiled with JetBrains decompiler
// Type: Meta.KalmanFilterClient
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class KalmanFilterClient
  {
    private float m_KalmanVelocity = 100f;
    private int m_KalmanID = -1;
    private int m_KalmanIDRot = -1;
    private int m_KalmanIDW = -1;
    private float xrot;
    private float yrot;
    private float zrot;
    private float wrot;
    private float dummy;
    private float dummy2;

    public float kalmanVelocity
    {
      get
      {
        return this.m_KalmanVelocity;
      }
      set
      {
        this.m_KalmanVelocity = value;
      }
    }

    public void KalmanFilterSmoothTransform(Transform transform, out Vector3 position, out Quaternion rotation)
    {
      bool flag = false;
      if (this.m_KalmanID == -1)
      {
        KalmanFilter.GetNewID(ref this.m_KalmanID, true, this.m_KalmanVelocity);
        KalmanFilter.GetNewID(ref this.m_KalmanIDRot, true, this.m_KalmanVelocity);
        KalmanFilter.GetNewID(ref this.m_KalmanIDW, true, this.m_KalmanVelocity);
        flag = true;
      }
      Quaternion rotation1 = transform.get_rotation();
      if (flag)
      {
        KalmanFilter.InitKalman(this.m_KalmanIDRot, (float) rotation1.x, (float) rotation1.y, (float) rotation1.z);
        KalmanFilter.InitKalman(this.m_KalmanIDW, (float) rotation1.w, 0.0f, 0.0f);
      }
      this.xrot = (float) rotation1.x;
      this.yrot = (float) rotation1.y;
      this.zrot = (float) rotation1.z;
      this.wrot = (float) rotation1.w;
      this.dummy = 0.0f;
      this.dummy2 = 0.0f;
      KalmanFilter.UpdateKalman(this.m_KalmanIDRot, ref this.xrot, ref this.yrot, ref this.zrot, this.m_KalmanVelocity);
      KalmanFilter.UpdateKalman(this.m_KalmanIDW, ref this.wrot, ref this.dummy, ref this.dummy2, this.m_KalmanVelocity);
      if (!float.IsNaN(this.xrot))
      {
        // ISSUE: explicit reference operation
        ((Quaternion) @rotation).\u002Ector(this.xrot, this.yrot, this.zrot, this.wrot);
      }
      else
      {
        Debug.LogError((object) "UpdateTransform: Quaternion.x is NaN.");
        rotation = transform.get_rotation();
      }
      Vector3 position1 = transform.get_position();
      if (flag)
        KalmanFilter.InitKalman(this.m_KalmanID, (float) position1.x, (float) position1.y, (float) position1.z);
      float x = (float) position1.x;
      float y = (float) position1.y;
      float z = (float) position1.z;
      KalmanFilter.UpdateKalman(this.m_KalmanID, ref x, ref y, ref z, this.m_KalmanVelocity);
      // ISSUE: explicit reference operation
      ((Vector3) @position).\u002Ector(x, y, z);
    }
  }
}
