using UnityEngine;
using System.Collections;

public class FollowPlayerCamera : MonoBehaviour {
	public int id;
	public void OnFollowPlayerInfo(FollowPlayerInfo playerInfo){
		if(playerInfo.playerID==id){
			transform.position=playerInfo.position;
			transform.rotation=Quaternion.Euler(playerInfo.rotation);
		}
	}
}
