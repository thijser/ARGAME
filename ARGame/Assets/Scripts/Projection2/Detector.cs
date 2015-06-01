namespace Projection{
	using UnityEngine;
	using System.Collections;

	public class Detector : MonoBehaviour {
		IARLink link;

		void start(){
			link =  new MetaLink();
		}
		void LateUpdate () {
			
		}
	}
}