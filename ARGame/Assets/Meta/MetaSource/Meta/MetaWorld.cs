using System;
using UnityEngine;

namespace Meta
{
	[ExecuteInEditMode]
	public class MetaWorld : MonoBehaviour
	{
		private void OnEnable()
		{
			MetaPlugin.Load();
			if (UnityEngine.Object.FindObjectsOfType<MetaWorld>().Length > 1)
			{
                UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}
	}
}
