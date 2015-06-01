using UnityEngine;
using System.Collections;
using Meta;
namespace Projection
{

public class MetaMarker : MonoBehaviour , MetaMarkerInterface{
		/// <summary>
		/// meta object required for tracking 
		/// </summary>
		private GameObject markerdetectorGO;
		
		/// <summary>
		/// meta object required for tracking 
		/// </summary>
		private MarkerTargetIndicator marketTargetindicator;


		/// <summary>
		/// sets up the detector and marker indicator to find this marker/ 
		/// </summary>
		public void RegisterMeta(){
			this.markerdetectorGO = MarkerDetector.Instance.gameObject;
			
			// hide markerindicator
			this.marketTargetindicator = this.markerdetectorGO.GetComponent<MarkerTargetIndicator>();
			this.marketTargetindicator.enabled = false;
			
		}
		///<summary>
		///orignally from the metaExample script, heavily edited and returns true 
		///if the marker was seen if it is then it also moves the object to the marker position 
		///</summary>
		public bool MoveTransformToMarker(int id,Transform trans){
			if (!this.markerdetectorGO.activeSelf)
			{
				this.markerdetectorGO.SetActive(true);
			}
			if (MarkerDetector.Instance != null)
			{
				// check if we can see this marker 
				if (MarkerDetector.Instance.updatedMarkerTransforms.Contains(id))
				{
					// if we can then move this marker to that position 
					MarkerDetector.Instance.GetMarkerTransform(id, ref trans);
					return true;
				}
			}
			return false;
		}
}
}