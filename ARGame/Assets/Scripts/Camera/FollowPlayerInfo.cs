using UnityEngine;
using System.Collections;
using Projection;
using Network;
public class FollowPlayerInfo : MonoBehaviour{
	public int id;
	void OnARViewUpdate(ARViewUpdate update){
		//if(update.ID==id){ remove comments to reanable 
			transform.position=update.Position;
			transform.rotation=Quaternion.Euler(update.Rotation);
		//}
	}
}
