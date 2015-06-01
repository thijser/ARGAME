namespace Projection{
	using UnityEngine;
	using System.Collections.Generic;

	public class Detector : MonoBehaviour {
		IARLink link;

		void start(){
			link =  new MetaLink();
		}
		void LateUpdate () {
			List<MarkerPosition> list = link.GetMarkerPositions();
			foreach( MarkerPosition mp in list){

			}
		}
	}
}