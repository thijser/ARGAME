using UnityEngine;
using System.Collections;
using Network;
public class FollowPlayerCamera : MonoBehaviour
{
    public int id;
    public void OnFollowPlayerInfo(ARViewUpdate playerInfo)
    {
       // if (playerInfo.Id == id)
       // {
            transform.position = playerInfo.Position;
            transform.rotation = Quaternion.Euler(playerInfo.Rotation);
		Debug.Log (playerInfo.Position);
		Debug.Log (playerInfo.Rotation);
        //}
    }
}
