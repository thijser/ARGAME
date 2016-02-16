using System;
using UnityEngine;

namespace Meta
{
	internal static class MetaUtils
	{
		public static void QuitApp()
		{
			Application.Quit();
		}

		public static Rect Vector4toRect(Vector4 v)
		{
			Rect result = new Rect(v.x, v.y, v.z, v.w);
			return result;
		}

		public static Vector4 RectToVector4(Rect r)
		{
			Vector4 result = new Vector4(r.get_x(), r.get_y(), r.get_width(), r.get_height());
			return result;
		}

		public static void FloatToVector3(float[] data, ref Vector3 vector)
		{
			vector.Set(data[0], data[1], data[2]);
		}

		public static Vector3 FloatToVector3(float[] data)
		{
			return new Vector3(data[0], data[1], data[2]);
		}

		public static void FloatToVector2(float[] data, ref Vector2 vector)
		{
			vector.Set(data[0], data[1]);
		}

		public static Vector2 FloatToVector2(float[] data)
		{
			return new Vector2(data[0], data[1]);
		}
	}
}
