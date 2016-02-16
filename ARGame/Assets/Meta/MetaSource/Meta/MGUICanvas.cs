using System;
using UnityEngine;

namespace Meta
{
	internal class MGUICanvas : MonoBehaviour
	{
		private void Start()
		{
			base.GetComponent<Canvas>().worldCamera = UnityEngine.Camera.main;
		}

		private void Update()
		{
		}
	}
}
