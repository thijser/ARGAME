namespace Projection{
	using UnityEngine;
	using System.Collections.Generic;

	public class Detector : MonoBehaviour {
		IARLink link;

		void Start(){
			link =  new MetaLink();
		}
		void LateUpdate () {
			List<MarkerPosition> list = link.GetMarkerPositions();
			foreach( MarkerPosition mp in list){
				this.SendMessage(
					"OnMarkerSeen", 
					mp, 
					SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}