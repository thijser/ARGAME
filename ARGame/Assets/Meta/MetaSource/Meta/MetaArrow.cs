using System;
using UnityEngine;

namespace Meta
{
	public class MetaArrow : MetaSingleton<MetaArrow>
	{
		public Transform targetTransform;

		public bool twoD = true;

		public bool alwaysOn;

		public bool IsVisibleFrom(Renderer renderer, UnityEngine.Camera camera)
		{
			if (renderer != null)
			{
				Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
				return GeometryUtility.TestPlanesAABB(array, renderer.bounds);
			}
			return true;
		}

		public bool IsVisible(Renderer targetRenderer)
		{
			bool result;
			if (MetaCamera.GetCameraMode() == CameraType.Monocular)
			{
				result = this.IsVisibleFrom(targetRenderer, UnityEngine.Camera.main);
			}
			else
			{
				result = (GameObject.Find("MetaCameraLeft") != null && GameObject.Find("MetaCameraRight") != null && (this.IsVisibleFrom(targetRenderer, GameObject.Find("MetaCameraLeft").GetComponent<UnityEngine.Camera>()) || this.IsVisibleFrom(targetRenderer, GameObject.Find("MetaCameraRight").GetComponent<UnityEngine.Camera>())));
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
			return this.IsVisible(tTransform.gameObject);
		}

		private void Start()
		{
			base.GetComponent<Renderer>().enabled = false;
		}

		private void Update()
		{
			if (this.targetTransform != null && this.targetTransform.gameObject.activeInHierarchy)
			{
				if (this.alwaysOn || !this.IsVisible(this.targetTransform))
				{
					base.GetComponent<Renderer>().enabled = true;
					Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Renderer renderer = componentsInChildren[i];
						renderer.enabled = true;
					}
					if (this.twoD)
					{
						Vector3 vector = this.targetTransform.position - base.transform.position;
						vector = Quaternion.Inverse(UnityEngine.Camera.main.transform.rotation) * vector;
						vector.Normalize();
						float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
						base.transform.localRotation = Quaternion.Euler(0f, 0f, num - 90f);
					}
					else
					{
						base.transform.LookAt(this.targetTransform);
					}
				}
				else
				{
					base.GetComponent<Renderer>().enabled = false;
					Renderer[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						Renderer renderer2 = componentsInChildren2[j];
						renderer2.enabled = false;
					}
				}
			}
			else
			{
				base.GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
