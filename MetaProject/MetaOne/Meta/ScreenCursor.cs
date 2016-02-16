using System;
using System.Reflection;
using UnityEngine;

namespace Meta
{
	internal static class ScreenCursor
	{
		private static int _version = int.Parse(Application.get_unityVersion()[0].ToString());

		internal static void SetMouseCursorVisibility(bool visibility)
		{
			if (ScreenCursor._version >= 5)
			{
				Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
				PropertyInfo property = type.GetProperty("visible");
				property.SetValue(type, visibility, null);
			}
			else
			{
				Type type2 = Types.GetType("UnityEngine.Screen", "UnityEngine");
				PropertyInfo property2 = type2.GetProperty("showCursor");
				property2.SetValue(type2, visibility, null);
			}
		}

		internal static void SetMouseCursorLockState(bool locked)
		{
			if (ScreenCursor._version >= 5)
			{
				Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
				PropertyInfo property = type.GetProperty("lockState");
				if (locked)
				{
					property.SetValue(type, 0, null);
				}
				else
				{
					property.SetValue(type, 1, null);
				}
			}
			else
			{
				Type type2 = Types.GetType("UnityEngine.Screen", "UnityEngine");
				PropertyInfo property2 = type2.GetProperty("lockCursor");
				property2.SetValue(type2, locked, null);
			}
		}

		internal static bool GetMouseCursorVisibility()
		{
			if (ScreenCursor._version >= 5)
			{
				Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
				PropertyInfo property = type.GetProperty("visible");
				return (bool)property.GetValue(type, null);
			}
			Type type2 = Types.GetType("UnityEngine.Screen", "UnityEngine");
			PropertyInfo property2 = type2.GetProperty("showCursor");
			return (bool)property2.GetValue(type2, null);
		}

		internal static bool GetMouseCursorLockState()
		{
			if (ScreenCursor._version >= 5)
			{
				Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
				PropertyInfo property = type.GetProperty("lockState");
				return (int)property.GetValue(type, null) != 0;
			}
			Type type2 = Types.GetType("UnityEngine.Screen", "UnityEngine");
			PropertyInfo property2 = type2.GetProperty("lockCursor");
			return (bool)property2.GetValue(type2, null);
		}
	}
}
