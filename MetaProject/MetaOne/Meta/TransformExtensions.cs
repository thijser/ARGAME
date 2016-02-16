using System;
using UnityEngine;

namespace Meta
{
	internal static class TransformExtensions
	{
		public static GameObject FindChildGameObjectOrDie(this Transform t, string gameObjectName)
		{
			Transform transform = t.Find(gameObjectName);
			GameObject gameObject;
			if (transform == null)
			{
				Debug.LogError("No " + gameObjectName + " GameObject found...");
				gameObject = null;
			}
			else if (!transform.get_gameObject().get_activeSelf())
			{
				Debug.LogError(gameObjectName + " found but it is inactive...");
				gameObject = null;
			}
			else
			{
				gameObject = transform.get_gameObject();
			}
			if (gameObject == null)
			{
				MetaUtils.QuitApp();
			}
			return gameObject;
		}
	}
}
