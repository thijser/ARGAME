using UnityEngine;
using System.Collections;

public class PositionLinker : MonoBehaviour {

	// Use this for initialization
	public Transform LinkedTo;
	public int linkedMode = 3;
	public Transform levelMarker;
	// Update is called once per frame
	void Update () {
		LinkedTo.position = transform.position;
		switch (linkedMode){
		case 0: //perfect copy
			LinkedTo.rotation = transform.rotation;
			break;
		case 1: 
			Vector3 EA = transform.rotation.eulerAngles;	//allow rotation by card but not in height 
			EA.x=LinkedTo.eulerAngles.x;
			EA.z=LinkedTo.eulerAngles.z;
			LinkedTo.rotation=Quaternion.Euler(EA);
			break;
		case 2: break; //ignore position of card and keep default
		case 3: //follow rotation of the level marker
			LinkedTo.rotation=levelMarker.rotation;
			break;
		}
	}
}
