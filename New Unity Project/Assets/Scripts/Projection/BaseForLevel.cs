using UnityEngine;
using System.Collections;

public class BaseForLevel : MonoBehaviour {
	GameObject basepoint;
	public long timestamp=0;
	int PATIENCE=10;
	public void seen(){
		timestamp=Time.frameCount;
		UsedCardManager holder = basepoint.GetComponent<UsedCardManager>();
		if(holder.currentlyUsed.timestamp+PATIENCE<timestamp){
			Transform p = transform.parent;
			transform.parent=null;
			p.parent=transform;
			holder.currentlyUsed=this;
		}
	}

}
