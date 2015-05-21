namespace Projection
{
    using System.Collections.Generic;
    using Network;
    using UnityEngine;

    public class RemoteObjectSyncer : MonoBehaviour 
    {
    	private Dictionary<int,GameObject> objectTable;
    	public List<GameObject> RegisterObjectsOnStartup;

    	public void SyncLoc(PositionUpdate update)
        {
    		if(!objectTable.ContainsKey(update.ID))
            {
    			throw new KeyNotFoundException("ID of not yet registered object");
    		}

    		GameObject toMove = objectTable[update.ID];
    		Transform transToMove = toMove.GetComponent<Transform>();
    		transToMove.localPosition = new Vector3(update.X,0,update.Y);
    	}

    	public void RegisterObject(int id,GameObject obj)
        {
    		objectTable.Add(id,obj);
    	}
    	
        public void Start() 
        {
            objectTable = new Dictionary<int,GameObject>();
    		int i=0;
    		foreach(GameObject go in RegisterObjectsOnStartup)
            {
    			i++;
    			objectTable.Add(i,go);
    			Transform transGo = (Transform)(go.GetComponent<Transform>());
    			transGo.SetParent(transform.parent,false);
    		}
    	}
    }
}
