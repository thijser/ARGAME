using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RemoteObjectSyncer : MonoBehaviour {
	public  Dictionary<int,GameObject> objectTable;
	public List<GameObject> RegisterObjectsOnStartup;

	public void SyncLoc (PositionUpdate P){
		if(!objectTable.ContainsKey(P.ID)){
			throw new KeyNotFoundException("ID of not yet registered object");
		}
		GameObject toMove=objectTable[P.ID];
		Transform transToMove = toMove.GetComponent<Transform>();
		transToMove.localPosition = new Vector3(P.X,0,P.Y);
	}
	public void RegisterObject(int id,GameObject obj){
		objectTable.Add(id,obj);
	}
	// load all objects in list into memory; 
	void Start () {
		int i=0;
		foreach(GameObject go in RegisterObjectsOnStartup){
			i++;
			objectTable.Add(i,go);
			Transform transGo = (Transform)(go.GetComponent<Transform>());
			transGo.SetParent(transform.parent,false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
