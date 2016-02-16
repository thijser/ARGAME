using System;
using UnityEngine;

namespace Meta
{
	internal static class Matrix4x4Extensions
	{
		public static Vector3 ScaleFromMatrix(this Matrix4x4 m)
		{
			Vector3 zero = Vector3.get_zero();
			for (int i = 0; i < 3; i++)
			{
				zero.set_Item(i, m.GetColumn(i).get_magnitude());
			}
			return zero;
		}

		public static Quaternion QuaternionFromMatrix(this Matrix4x4 m)
		{
			Vector3 vector = m.ScaleFromMatrix();
			for (int i = 0; i < 3; i++)
			{
				m.SetColumn(i, m.GetColumn(i) / vector.get_Item(i));
			}
			Quaternion result = default(Quaternion);
			result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + m.get_Item(0, 0) + m.get_Item(1, 1) + m.get_Item(2, 2))) / 2f;
			result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + m.get_Item(0, 0) - m.get_Item(1, 1) - m.get_Item(2, 2))) / 2f;
			result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - m.get_Item(0, 0) + m.get_Item(1, 1) - m.get_Item(2, 2))) / 2f;
			result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - m.get_Item(0, 0) - m.get_Item(1, 1) + m.get_Item(2, 2))) / 2f;
			result.x *= Mathf.Sign(result.x * (m.get_Item(2, 1) - m.get_Item(1, 2)));
			result.y *= Mathf.Sign(result.y * (m.get_Item(0, 2) - m.get_Item(2, 0)));
			result.z *= Mathf.Sign(result.z * (m.get_Item(1, 0) - m.get_Item(0, 1)));
			return result;
		}

		public static Vector3 PositionFromMatrix(this Matrix4x4 m)
		{
			return m.GetColumn(3);
		}
	}
}
