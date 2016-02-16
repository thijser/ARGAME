using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	internal class KalmanFilter
	{
		private static List<int> _IdList = new List<int>();

		private static List<Transform> previousTransform = new List<Transform>();

		public static float positionDelta;

		public static float rotationDeltaDegrees;

		public static float positionDeltaToKalmanMultiplier = 0f;

		public static float m_KalmanVelocity = 8f;

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
			int num = KalmanFilter._IdList.Count - 1;
			if (TransformID == -1)
			{
				TransformID = num / 3 + 1;
				KalmanFilter._IdList.Add(-1);
				KalmanFilter._IdList.Add(-1);
				KalmanFilter._IdList.Add(-1);
				int value = -1;
				int value2 = -1;
				int value3 = -1;
				KalmanFilter.GetNewID(ref value, true, 0f);
				KalmanFilter.GetNewID(ref value2, true, 0f);
				KalmanFilter.GetNewID(ref value3, true, 0f);
				KalmanFilter._IdList[num] = value2;
				KalmanFilter._IdList[num + 1] = value3;
				KalmanFilter._IdList[num + 2] = value;
				KalmanFilter.InitKalman(KalmanFilter._IdList[num], rotation.x, rotation.y, rotation.z);
				KalmanFilter.InitKalman(KalmanFilter._IdList[num + 1], rotation.w, 0f, 0f);
				KalmanFilter.InitKalman(KalmanFilter._IdList[num + 2], position.x, position.y, position.z);
				return;
			}
			KalmanFilter.xrot = rotation.x;
			KalmanFilter.yrot = rotation.y;
			KalmanFilter.zrot = rotation.z;
			KalmanFilter.wrot = rotation.w;
			KalmanFilter.dummy = 0f;
			KalmanFilter.dummy2 = 0f;
			KalmanFilter.x = position.x;
			KalmanFilter.y = position.y;
			KalmanFilter.z = position.z;
			if (dynamicParameters)
			{
				KalmanFilter.positionDelta = Vector3.Distance(KalmanFilter.previousTransform[TransformID].get_localPosition(), kalmanTransform.get_localPosition());
				KalmanFilter.m_KalmanVelocity = KalmanFilter.positionDelta * KalmanFilter.positionDeltaToKalmanMultiplier * Time.get_deltaTime();
				KalmanFilter.rotationDeltaDegrees = Quaternion.Angle(KalmanFilter.previousTransform[TransformID].get_localRotation(), kalmanTransform.get_localRotation());
			}
			if (KalmanFilter.positionDelta >= 0.01f)
			{
				KalmanFilter.previousTransform[TransformID].set_localPosition(kalmanTransform.get_localPosition());
			}
			else
			{
				KalmanFilter.m_KalmanVelocity = 0.001f;
			}
			if (KalmanFilter.rotationDeltaDegrees >= 0.01f)
			{
				KalmanFilter.previousTransform[TransformID].set_localRotation(kalmanTransform.get_localRotation());
			}
			KalmanFilter.UpdateKalman(KalmanFilter._IdList[num + 2], ref KalmanFilter.x, ref KalmanFilter.y, ref KalmanFilter.z, KalmanFilter.m_KalmanVelocity);
			KalmanFilter.UpdateKalman(KalmanFilter._IdList[num + 1], ref KalmanFilter.xrot, ref KalmanFilter.yrot, ref KalmanFilter.zrot, KalmanFilter.m_KalmanVelocity);
			KalmanFilter.UpdateKalman(KalmanFilter._IdList[num], ref KalmanFilter.wrot, ref KalmanFilter.dummy, ref KalmanFilter.dummy2, KalmanFilter.m_KalmanVelocity);
			if (!float.IsNaN(KalmanFilter.xrot))
			{
				kalmanTransform.set_rotation(new Quaternion(KalmanFilter.xrot, KalmanFilter.yrot, KalmanFilter.zrot, KalmanFilter.wrot));
			}
			else
			{
				Debug.Log("ERROR: UpdateTransform: transform.rotation = new Quaternion(xrot, yrot, zrot, wrot); // xrot can't be NaN");
			}
			kalmanTransform.set_position(new Vector3(KalmanFilter.x, KalmanFilter.y, KalmanFilter.z));
		}
	}
}
