using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Network;
using Projection;
using UnityEngine.Assertions;

public class HeadHolder : MonoBehaviour {

	private Dictionary<int,GameObject> Heads = new Dictionary<int, GameObject>();

	private RemoteMarkerHolder holder;

	private GameObject PrefabHead;

	public Vector2 BoardSize { get; set; }

	public void Start(){
		PrefabHead= (GameObject)Resources.Load("Prefabs/HEAD");
		holder=gameObject.GetComponent<RemoteMarkerHolder>();
	}

	/// <summary>
	/// The factor with which to scale the position.
	/// </summary>
	public  float ScaleFactor = 1f/8f;

	public Vector3 RotationOffset=new Vector3(0,0,0);
	public Vector3 LinairOffset = new Vector3(0,0,0);

	public void OnFollowPlayerInfo(ARViewUpdate playerInfo){
		place (this.HeadInstance(playerInfo.Id),playerInfo);
	}

	private GameObject HeadInstance(int id){
		if(!Heads.ContainsKey(id)){
			Heads.Add(id,Instantiate(PrefabHead));
			Heads[id].transform.SetParent(transform);
			RemoteMarker rm = Heads[id].GetComponent<RemoteMarker>();
			rm.Id=9000+id;
		}
		return Heads[id];
	}
	public void place(GameObject head,ARViewUpdate playerInfo){
		Debug.Log (" I know where you sleep");
		RemoteMarker rm = head.GetComponent<RemoteMarker>();
		rm.RemotePosition.Position=playerInfo.Position;
		rm.RemotePosition.Rotation=Quaternion.Euler(playerInfo.Rotation);
	}

	/// <summary>
	/// Updates the board size.
	/// </summary>
	/// <param name="update">The level update.</param>
	public void OnLevelUpdate(LevelUpdate update)
	{
		this.BoardSize = update.Size;
	}
}
