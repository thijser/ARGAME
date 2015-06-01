using UnityEngine;
using System.Collections;
namespace Projection{
	public class Marker : MonoBehaviour {
		public int id=-1;
		public MarkerPosition remotePosition;
		public MarkerPosition localPosition;
		float objectRotation; 
	
		public void SetObjectRotation(float rotation){
			objectRotation=rotation;
		}
	
		public void SetRemotePosition(MarkerPosition rem){
			remotePosition=rem;
		}
		public void SetLocalPosition(MarkerPosition rem){
			localPosition=rem;
		}
		void Start(){
			this.SendMessageUpwards("OnMarkerRegister",new MarkerRegister(this));
		}
	}
}