using UnityEngine;
using System.Collections;

public class RotCam : MonoBehaviour {
	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		onArrowRotate();
	}

	void onArrowRotate(){
		if(Input.GetKey(KeyCode.RightArrow)){
			transform.Rotate(transform.forward*-1*speed*Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.LeftArrow)){
			transform.Rotate(transform.forward*speed*Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			transform.Rotate(transform.right*speed*Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.DownArrow)){
				transform.Rotate(-1*transform.right*speed*Time.deltaTime);
		}
	}
}
