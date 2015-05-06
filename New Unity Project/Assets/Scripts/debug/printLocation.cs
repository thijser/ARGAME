using UnityEngine;
using System.Collections;

public class printLocation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (" hello");
		Debug.Log ("position:"  + transform.position);
		Debug.Log ("rotation:" + transform.rotation);
	}
}
