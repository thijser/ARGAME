using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Meta;
using System;
namespace Projection{
public class MetaLink : MonoBehaviour ,IARLink{
		/// <summary>
		/// like cattle this class is driven all around it's very position consumed by the meta, please no cowtipping with the lamb
		/// </summary>
		private GameObject lamb=new GameObject(); 
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
		public void Start(){
			this.markerdetectorGO = MarkerDetector.Instance.gameObject;
			
			// hide markerindicator
			this.marketTargetindicator = this.markerdetectorGO.GetComponent<MarkerTargetIndicator>();
			this.marketTargetindicator.enabled = false;
			
		}
		public void ensureMeta(){
			if (!this.markerdetectorGO.activeSelf)
			{
				this.markerdetectorGO.SetActive(true);
			}
			if (MarkerDetector.Instance == null){
				throw new MissingComponentException("All out of MarkerDetectors, I'm very very sorry");
			}
		}
		public List<MarkerPosition>GetMarkerPositions(){
				ensureMeta ();
				Transform trans = lamb.transform;
				List<MarkerPosition> list = new List<MarkerPosition>();
				foreach (int ID in MarkerDetector.Instance.updatedMarkerTransforms){
					MarkerDetector.Instance.GetMarkerTransform(ID,ref trans);
					MarkerPosition MP = new MarkerPosition(trans.position,trans.rotation,DateTime.Now,trans.localScale);
					list.Add(MP);
				}
			return list;
	}
}

}