using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	public class MarkerTargetIndicator : MonoBehaviour
	{
		[SerializeField]
		private GameObject _markerIndicatorBase;

		[SerializeField]
		private bool _isController;

		public static Dictionary<int, GameObject> markerIndicators = new Dictionary<int, GameObject>();

		private static bool _indicatorsVisible = true;

		private static GameObject _indicatorParent;

		public static bool indicatorsVisible
		{
			get
			{
				return MarkerTargetIndicator._indicatorsVisible;
			}
			set
			{
				if (MarkerTargetIndicator._indicatorsVisible != value && MarkerTargetIndicator._indicatorParent != null)
				{
					MarkerTargetIndicator._indicatorParent.SetActive(value);
					MarkerTargetIndicator._indicatorsVisible = value;
				}
			}
		}

		public static KeyValuePair<int, GameObject> GetClosestMarkerIndicator(Transform gameObj, ref float distance)
		{
			KeyValuePair<int, GameObject> result = new KeyValuePair<int, GameObject>(-1, null);
			float num = MetaSingleton<MarkerDetector>.Instance.markerReleaseRange;
			foreach (KeyValuePair<int, GameObject> current in MarkerTargetIndicator.markerIndicators)
			{
				float num2 = Vector3.Distance(current.Value.transform.position, gameObj.position);
				if (num2 < num)
				{
					num = num2;
					result = current;
				}
			}
			distance = num;
			return result;
		}

		public static void UpdateClosestMarkerID(Transform source, bool active)
		{
			MetaBody component = source.GetComponent<MetaBody>();
			if (component != null && component.markerTarget && component.markerTargetPlaceable)
			{
				float distance = -1f;
				KeyValuePair<int, GameObject> closestMarkerIndicator = MarkerTargetIndicator.GetClosestMarkerIndicator(source, ref distance);
				if (MarkerTargetIndicator.indicatorsVisible && component.markerTargetID != -1 && (component.markerTargetID != closestMarkerIndicator.Key || !active))
				{
					MarkerTargetIndicator.UnhighlightMarker(component.markerTargetID);
				}
				component.markerTargetID = closestMarkerIndicator.Key;
				if (MarkerTargetIndicator.indicatorsVisible && closestMarkerIndicator.Key != -1 && component.markerTargetPlaceableHighlight && active)
				{
					MarkerTargetIndicator.HighlightMarker(component.markerTargetID, distance);
				}
			}
		}

		public static void HighlightMarker(int markerID, float distance)
		{
			if (MarkerTargetIndicator.markerIndicators.ContainsKey(markerID))
			{
				GameObject gameObject = MarkerTargetIndicator.markerIndicators[markerID];
				HighlightableObject highlightableObject = gameObject.GetComponent<HighlightableObject>();
				if (highlightableObject == null)
				{
					highlightableObject = gameObject.AddComponent<HighlightableObject>();
				}
				highlightableObject.enabled = true;
				float num = 0.05f;
				float markerReleaseRange = MetaSingleton<MarkerDetector>.Instance.markerReleaseRange;
				float num2 = 1f - (distance - num) / (markerReleaseRange - num);
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				highlightableObject.ConstantOnImmediate(new Color(1f, 1f, 1f, num2));
			}
		}

		public static void UnhighlightMarker(int markerID)
		{
			if (MarkerTargetIndicator.markerIndicators.ContainsKey(markerID))
			{
				GameObject gameObject = MarkerTargetIndicator.markerIndicators[markerID];
				HighlightableObject component = gameObject.GetComponent<HighlightableObject>();
				if (component != null)
				{
					component.ConstantOff();
				}
			}
		}

		private void UpdateMarkerIndicators()
		{
			foreach (int current in MetaSingleton<MarkerDetector>.Instance.updatedMarkerTransforms)
			{
				if (!MarkerTargetIndicator.markerIndicators.ContainsKey(current))
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this._markerIndicatorBase);
					if (MarkerTargetIndicator._indicatorParent == null)
					{
						MarkerTargetIndicator._indicatorParent = new GameObject();
						MarkerTargetIndicator._indicatorParent.name = "MarkerIndicators";
						MarkerTargetIndicator._indicatorParent.transform.position = Vector3.zero;
						MarkerTargetIndicator._indicatorParent.transform.rotation = Quaternion.identity;
					}
					gameObject.name = "MarkerIndicator" + current;
					gameObject.transform.parent = MarkerTargetIndicator._indicatorParent.transform;
					Texture2D mainTexture = Resources.Load("Markers/AprilTags/tag36_11_" + current.ToString("D5")) as Texture2D;
					if (gameObject.GetComponentsInChildren<Renderer>(true).Length > 0)
					{
						gameObject.GetComponentsInChildren<Renderer>(true)[0].material.mainTexture = mainTexture;
					}
					MarkerTarget component = gameObject.GetComponent<MarkerTarget>();
					if (component != null)
					{
						component.id = current;
					}
					MarkerTargetIndicator.markerIndicators.Add(current, gameObject);
				}
				Transform transform = MarkerTargetIndicator.markerIndicators[current].transform;
				MetaSingleton<MarkerDetector>.Instance.GetMarkerTransform(current, ref transform);
			}
		}

		private void Awake()
		{
			if (MarkerTargetIndicator._indicatorParent == null)
			{
				MarkerTargetIndicator._indicatorParent = new GameObject();
				MarkerTargetIndicator._indicatorParent.name = "MarkerIndicators";
				MarkerTargetIndicator._indicatorParent.transform.position = Vector3.zero;
				MarkerTargetIndicator._indicatorParent.transform.rotation = Quaternion.identity;
			}
		}

		private void Start()
		{
			if (this._isController)
			{
				MarkerTargetIndicator.markerIndicators = new Dictionary<int, GameObject>();
			}
		}

		private void Update()
		{
			if (this._isController && MetaSingleton<MarkerDetector>.Instance != null && MetaSingleton<MarkerDetector>.Instance.enabled)
			{
				this.UpdateMarkerIndicators();
			}
		}

		private void OnEnable()
		{
			if (MarkerTargetIndicator._indicatorParent != null)
			{
				MarkerTargetIndicator._indicatorParent.SetActive(true);
			}
		}

		private void OnDisable()
		{
			if (MarkerTargetIndicator._indicatorParent != null)
			{
				MarkerTargetIndicator._indicatorParent.SetActive(false);
			}
		}
	}
}
