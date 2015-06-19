using UnityEngine;
using System.Collections;
using Projection;
public class FollowPlayerInfo {

	public Vector3 position{get;set;}
	public Vector3 rotation{get;set;}
	public int playerID{get;set;}
	private Transform lamb;
 	void Start(){
		GameObject l= new GameObject("cattle");
		lamb=l.transform;
	}
	public FollowPlayerInfo(int id,MarkerHolder mh){

		Marker parent=mh.Parent;
		Matrix4x4 posmatrix=parent.TransformMatrix.inverse;
		lamb.SetFromMatrix(posmatrix);
		position=lamb.position;
		rotation=lamb.rotation.eulerAngles;
		playerID=id;
	}
}
