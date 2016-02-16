using System;
using UnityEngine;

namespace Meta
{
	internal class MGUICanvas : MonoBehaviour
	{
		private void Start()
		{
			base.GetComponent<Canvas>().set_worldCamera(Camera.get_main());
		}

		private void Update()
		{
		}
	}
}
