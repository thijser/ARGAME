using System;
using UnityEngine;

namespace Meta
{
	internal class MetaLocalizationHotKey : MonoBehaviour
	{
		private MetaLocalization metaLocalization;

		private void Start()
		{
			if (this.metaLocalization == null)
			{
				this.metaLocalization = base.GetComponent<MetaLocalization>();
			}
		}

		private void Update()
		{
			if (this.metaLocalization == null)
			{
				this.metaLocalization = base.GetComponent<MetaLocalization>();
			}
			if (this.metaLocalization != null && MetaSingleton<KeyboardShortcuts>.Instance.recalibrate != string.Empty && Input.GetKeyDown(MetaSingleton<KeyboardShortcuts>.Instance.recalibrate))
			{
				this.metaLocalization.ResetLocalizer();
			}
		}
	}
}
