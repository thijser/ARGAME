using UnityEngine;
using System.Collections;
using Network;
public class FollowPlayerCamera : MonoBehaviour {
	public int id;
	public void OnFollowPlayerInfo(ARViewUpdate playerInfo){
		if(playerInfo.ID==id){
			transform.position=playerInfo.Position;
			transform.rotation=Quaternion.Euler(playerInfo.Rotation);
		}
	}
}
