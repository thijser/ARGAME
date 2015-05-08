using UnityEngine;
using System.Collections;

public class PositionLinker : MonoBehaviour {

	// Use this for initialization
	public Transform LinkedTo;
	
	// Update is called once per frame
	void Update () {
		LinkedTo.position = transform.position;
		LinkedTo.rotation = transform.rotation;
	}
}
