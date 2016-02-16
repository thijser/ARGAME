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
			if (Object.FindObjectsOfType<MetaWorld>().Length > 1)
			{
				Object.DestroyImmediate(base.get_gameObject());
			}
		}
	}
}
