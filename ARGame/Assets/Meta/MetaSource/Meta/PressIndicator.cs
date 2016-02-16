using System;
using UnityEngine;

namespace Meta
{
	internal class PressIndicator : MonoBehaviour
	{
		private void Start()
		{
			base.gameObject.layer = LayerMask.NameToLayer("HUD");
			base.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
			LeanTween.alpha(base.gameObject, 0f, 0.75f);
			LeanTween.scale(base.gameObject, new Vector3(0.003f, 0.003f, 0.003f), 0.5f);
            UnityEngine.Object.Destroy(base.gameObject, 1f);
		}

		private void Update()
		{
		}
	}
}
