using UnityEngine;
using System.Collections;
using System;
using Network;
/// <summary>
/// Keeps data from either remote or local on the position of the marker.
/// 
/// </summary>
public class MarkerPosition : MonoBehaviour {
	public Vector3 Position;
	public Quaternion Rotation;
	public DateTime timeStamp;
	public Vector3 scale;


	/// <summary>
	/// create a new markerPosition from a position update 
	/// </summary>
	/// <param name="pu">Pu.</param>
	public MarkerPosition(PositionUpdate pu){
		this.scale=new Vector3(1,1,1);
		this.timeStamp=DateTime.Now;
		this.Position= new Vector3(pu.Coordinate[0],0,pu.Coordinate[1]);
		this.scale=new Vector3(1,1,1);
	}
	public MarkerPosition(Vector3 pos,Quaternion rot,DateTime stamp,Vector3 scale){
		Position=pos;
		Rotation=rot;
		timeStamp=stamp;
		this.scale=scale;
	}
}
