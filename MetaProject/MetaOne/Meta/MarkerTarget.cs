using System;
using UnityEngine;

namespace Meta
{
	public class MarkerTarget : MonoBehaviour
	{
		internal int id = -1;

		internal void MarkerTargetPersistentLoad()
		{
			MetaBody component = base.get_gameObject().GetComponent<MetaBody>();
			if (component != null && component.markerTargetPersistent)
			{
				component.markerTargetID = component.GetPersistentMarkerTargetID();
			}
		}

		internal void MarkerTargetPersistentSave()
		{
			MetaBody component = base.get_gameObject().GetComponent<MetaBody>();
			if (component != null && component.markerTargetPersistent)
			{
				component.SetPersistentMarkerTargetID(component.markerTargetID);
			}
		}

		private void Start()
		{
			this.MarkerTargetPersistentLoad();
		}

		private void LateUpdate()
		{
			MetaBody component = base.get_gameObject().GetComponent<MetaBody>();
			if (component == null || (!component.grabbed && !component.pinched))
			{
				Transform transform = base.get_transform();
				if (MetaSingleton<MarkerDetector>.Instance != null)
				{
					MetaSingleton<MarkerDetector>.Instance.GetMarkerTransform(this.id, ref transform);
				}
			}
		}

		private void OnDisable()
		{
			this.MarkerTargetPersistentSave();
		}
	}
}
