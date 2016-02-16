using System;
using UnityEngine;

namespace Meta
{
	[Obsolete("Not used anymore", true)]
	public class MetaSpace : MetaSingleton<MetaSpace>
	{
		public GameObject keyboardObject
		{
			get
			{
				Debug.LogError("MetaSpace is deprecated. Please use MetaKeybaord.Instance.keyboardObject instead");
				return null;
			}
		}

		public static void LoadScene(string sceneName, bool displayLoading = true)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaCore.LoadScene() instead");
		}

		public static void LoadScene(int sceneNum, bool displayLoading = true)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaCore.LoadScene() instead");
		}

		public static void LoadSceneAdditive(string sceneName, bool displayLoading = true)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaCore.LoadSceneAdditive() instead");
		}

		public static void LoadSceneAdditive(int sceneNum, bool displayLoading = true)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaCore.LoadSceneAdditive() instead");
		}

		public void RequestKeyboard(GameObject keyboardObject)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaKeyboard.Instance.RequestKeyboard() instead");
		}

		public void ReleaseKeyboard(GameObject keyboardObject)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaKeyboard.Instance.ReleaseKeyboard() instead");
		}

		public void SetKeyboardPosition(GameObject keyboardObject, Vector3 pos)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaKeyboard.Instance.SetKeyboardPosition() instead");
		}

		public void SetKeyboardRotation(GameObject keyboardObject, Quaternion rot)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaKeyboard.Instance.SetKeyboardRotation() instead");
		}

		public static bool IsVisible(Bounds bounds)
		{
			Debug.LogError("MetaSpace is deprecated. Please use MetaCamera.IsVisible() instead");
			return false;
		}
	}
}
