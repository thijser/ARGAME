using System;
using UnityEngine;

namespace Meta
{
	public class HandsInputModuleAdder : MonoBehaviour
	{
		private void Start()
		{
			if (base.gameObject.GetComponent<HandsInputModule>() == null)
			{
				base.gameObject.AddComponent<HandsInputModule>();
			}
		}
	}
}
