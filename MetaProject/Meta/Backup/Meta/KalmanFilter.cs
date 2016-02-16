// Decompiled with JetBrains decompiler
// Type: Meta.KalmanFilter
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  internal class KalmanFilter
  {
    private static List<int> _IdList = new List<int>();
    private static List<Transform> previousTransform = new List<Transform>();
    public static float positionDeltaToKalmanMultiplier = 0.0f;
    public static float m_KalmanVelocity = 8f;
    public static float positionDelta;
    public static float rotationDeltaDegrees;
    private static float xrot;
    private static float yrot;
    private static float zrot;
    private static float wrot;
    private static float x;
    private static float y;
    private static float z;
    private static float dummy;
    private static float dummy2;

    [DllImport("KalmanFilter", EntryPoint = "get_new_id")]
    public static extern void GetNewID(ref int id, bool useVelocity, float velocity);

    [DllImport("KalmanFilter", EntryPoint = "init_kalman")]
    public static extern void InitKalman(int id, float x, float y, float z);

    [DllImport("KalmanFilter", EntryPoint = "delete_id")]
    public static extern bool DeleteID(int id);

    [DllImport("KalmanFilter", EntryPoint = "get_id_count")]
    public static extern void GetIDCount(ref int num);

    [DllImport("KalmanFilter", EntryPoint = "update_kalman")]
    public static extern bool UpdateKalman(int id, ref float x, ref float y, ref float z, float velocity);

    [DllImport("KalmanFilter", EntryPoint = "reset_kalman")]
    public static extern bool ResetKalman(int id);

    public static void KalmanFilterSmoothTransform(ref Transform kalmanTransform, ref int TransformID, ref bool dynamicParameters)
    {
      Quaternion rotation = kalmanTransform.get_rotation();
      Vector3 position = kalmanTransform.get_position();
      int index = KalmanFilter._IdList.Count - 1;
      if (TransformID == -1)
      {
        TransformID = index / 3 + 1;
        KalmanFilter._IdList.Add(-1);
        KalmanFilter._IdList.Add(-1);
        KalmanFilter._IdList.Add(-1);
        int id1 = -1;
        int id2 = -1;
        int id3 = -1;
        KalmanFilter.GetNewID(ref id1, true, 0.0f);
        KalmanFilter.GetNewID(ref id2, true, 0.0f);
        KalmanFilter.GetNewID(ref id3, true, 0.0f);
        KalmanFilter._IdList[index] = id2;
        KalmanFilter._IdList[index + 1] = id3;
        KalmanFilter._IdList[index + 2] = id1;
        KalmanFilter.InitKalman(KalmanFilter._IdList[index], (float) rotation.x, (float) rotation.y, (float) rotation.z);
        KalmanFilter.InitKalman(KalmanFilter._IdList[index + 1], (float) rotation.w, 0.0f, 0.0f);
        KalmanFilter.InitKalman(KalmanFilter._IdList[index + 2], (float) position.x, (float) position.y, (float) position.z);
      }
      else
      {
        KalmanFilter.xrot = (float) rotation.x;
        KalmanFilter.yrot = (float) rotation.y;
        KalmanFilter.zrot = (float) rotation.z;
        KalmanFilter.wrot = (float) rotation.w;
        KalmanFilter.dummy = 0.0f;
        KalmanFilter.dummy2 = 0.0f;
        KalmanFilter.x = (float) position.x;
        KalmanFilter.y = (float) position.y;
        KalmanFilter.z = (float) position.z;
        if (dynamicParameters)
        {
          KalmanFilter.positionDelta = Vector3.Distance(KalmanFilter.previousTransform[TransformID].get_localPosition(), kalmanTransform.get_localPosition());
          KalmanFilter.m_KalmanVelocity = KalmanFilter.positionDelta * KalmanFilter.positionDeltaToKalmanMultiplier * Time.get_deltaTime();
          KalmanFilter.rotationDeltaDegrees = Quaternion.Angle(KalmanFilter.previousTransform[TransformID].get_localRotation(), kalmanTransform.get_localRotation());
        }
        if ((double) KalmanFilter.positionDelta >= 0.00999999977648258)
          KalmanFilter.previousTransform[TransformID].set_localPosition(kalmanTransform.get_localPosition());
        else
          KalmanFilter.m_KalmanVelocity = 1.0 / 1000.0;
        if ((double) KalmanFilter.rotationDeltaDegrees >= 0.00999999977648258)
          KalmanFilter.previousTransform[TransformID].set_localRotation(kalmanTransform.get_localRotation());
        KalmanFilter.UpdateKalman(KalmanFilter._IdList[index + 2], ref KalmanFilter.x, ref KalmanFilter.y, ref KalmanFilter.z, KalmanFilter.m_KalmanVelocity);
        KalmanFilter.UpdateKalman(KalmanFilter._IdList[index + 1], ref KalmanFilter.xrot, ref KalmanFilter.yrot, ref KalmanFilter.zrot, KalmanFilter.m_KalmanVelocity);
        KalmanFilter.UpdateKalman(KalmanFilter._IdList[index], ref KalmanFilter.wrot, ref KalmanFilter.dummy, ref KalmanFilter.dummy2, KalmanFilter.m_KalmanVelocity);
        if (!float.IsNaN(KalmanFilter.xrot))
          kalmanTransform.set_rotation(new Quaternion(KalmanFilter.xrot, KalmanFilter.yrot, KalmanFilter.zrot, KalmanFilter.wrot));
        else
          Debug.Log((object) "ERROR: UpdateTransform: transform.rotation = new Quaternion(xrot, yrot, zrot, wrot); // xrot can't be NaN");
        kalmanTransform.set_position(new Vector3(KalmanFilter.x, KalmanFilter.y, KalmanFilter.z));
      }
    }
  }
}
