using System;
using UnityEngine;

namespace Meta
{
	public class HandsInputModuleAdder : MonoBehaviour
	{
		private void Start()
		{
			if (base.get_gameObject().GetComponent<HandsInputModule>() == null)
			{
				base.get_gameObject().AddComponent<HandsInputModule>();
			}
			base.set_hideFlags(2);
		}
	}
}
