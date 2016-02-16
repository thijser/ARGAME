using System;
using UnityEngine;

namespace Meta
{
	public class MetaArrow : MetaSingleton<MetaArrow>
	{
		public Transform targetTransform;

		public bool twoD = true;

		public bool alwaysOn;

		public bool IsVisibleFrom(Renderer renderer, Camera camera)
		{
			if (renderer != null)
			{
				Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
				return GeometryUtility.TestPlanesAABB(array, renderer.get_bounds());
			}
			return true;
		}

		public bool IsVisible(Renderer targetRenderer)
		{
			bool result;
			if (MetaCamera.GetCameraMode() == CameraType.Monocular)
			{
				result = this.IsVisibleFrom(targetRenderer, Camera.get_main());
			}
			else
			{
				result = (GameObject.Find("MetaCameraLeft") != null && GameObject.Find("MetaCameraRight") != null && (this.IsVisibleFrom(targetRenderer, GameObject.Find("MetaCameraLeft").GetComponent<Camera>()) || this.IsVisibleFrom(targetRenderer, GameObject.Find("MetaCameraRight").GetComponent<Camera>())));
			}
			return result;
		}

		public bool IsVisible(GameObject targetObject)
		{
			Renderer[] componentsInChildren = targetObject.GetComponentsInChildren<Renderer>();
			bool result = false;
			Renderer[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Renderer targetRenderer = array[i];
				if (this.IsVisible(targetRenderer))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool IsVisible(Transform tTransform)
		{
			return this.IsVisible(tTransform.get_gameObject());
		}

		private void Start()
		{
			base.GetComponent<Renderer>().set_enabled(false);
		}

		private void Update()
		{
			if (this.targetTransform != null && this.targetTransform.get_gameObject().get_activeInHierarchy())
			{
				if (this.alwaysOn || !this.IsVisible(this.targetTransform))
				{
					base.GetComponent<Renderer>().set_enabled(true);
					Renderer[] componentsInChildren = base.get_gameObject().GetComponentsInChildren<Renderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Renderer renderer = componentsInChildren[i];
						renderer.set_enabled(true);
					}
					if (this.twoD)
					{
						Vector3 vector = this.targetTransform.get_position() - base.get_transform().get_position();
						vector = Quaternion.Inverse(Camera.get_main().get_transform().get_rotation()) * vector;
						vector.Normalize();
						float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
						base.get_transform().set_localRotation(Quaternion.Euler(0f, 0f, num - 90f));
					}
					else
					{
						base.get_transform().LookAt(this.targetTransform);
					}
				}
				else
				{
					base.GetComponent<Renderer>().set_enabled(false);
					Renderer[] componentsInChildren2 = base.get_gameObject().GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						Renderer renderer2 = componentsInChildren2[j];
						renderer2.set_enabled(false);
					}
				}
			}
			else
			{
				base.GetComponent<Renderer>().set_enabled(false);
			}
		}
	}
}
