using System;
using UnityEngine;

namespace Meta
{
	internal class PressIndicator : MonoBehaviour
	{
		private void Start()
		{
			base.get_gameObject().set_layer(LayerMask.NameToLayer("HUD"));
			base.get_transform().set_localScale(new Vector3(0.001f, 0.001f, 0.001f));
			LeanTween.alpha(base.get_gameObject(), 0f, 0.75f);
			LeanTween.scale(base.get_gameObject(), new Vector3(0.003f, 0.003f, 0.003f), 0.5f);
			Object.Destroy(base.get_gameObject(), 1f);
		}

		private void Update()
		{
		}
	}
}
