using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Network;
using Projection;
using UnityEngine.Assertions;

public class HeadHolder : MonoBehaviour {

	private Dictionary<int,GameObject> Heads = new Dictionary<int, GameObject>();

	private GameObject PrefabHead;

	public Vector2 BoardSize { get; set; }

	public void Start(){
		PrefabHead= (GameObject)Resources.Load("Prefabs/HEAD");
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
		}
		return Heads[id];
	}
	public void place(GameObject head,ARViewUpdate playerInfo){
		head.gameObject.SetActive(true);
/*		head.transform.localPosition = new Vector3(
			-(playerInfo.Position.x + HorizontalOffset) * ScaleFactor,
			 (playerInfo.Position.y + ZOffset)*ScaleFactor,
			(playerInfo.Position.y + VerticalOffset) * ScaleFactor);
		head.transform.rotation=Quaternion.Euler(playerInfo.Rotation+RotationOffset);
*/		head.transform.rotation=Quaternion.Euler(playerInfo.Rotation+RotationOffset);
		Vector3 p= playerInfo.Position*ScaleFactor;
		p.x=this.BoardSize.x-p.x;
		head.transform.localPosition = p + head.transform.rotation * this.LinairOffset;
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
