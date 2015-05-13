using UnityEngine;
using System.Collections;

public class ArrowKeyMove : MonoBehaviour {

	private float speed = 10f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		InputMovement ();
	}

	void InputMovement()
	{
		if (Input.GetKey (KeyCode.UpArrow))
			GetComponent<Transform> ().position = GetComponent<Transform> ().position + Vector3.forward * speed * Time.deltaTime;
		
		if (Input.GetKey(KeyCode.DownArrow))
			GetComponent<Transform> ().position = GetComponent<Transform> ().position - Vector3.forward * speed * Time.deltaTime;
		
		if (Input.GetKey(KeyCode.RightArrow))
			GetComponent<Transform> ().position = GetComponent<Transform> ().position + Vector3.right * speed * Time.deltaTime;
		
		if (Input.GetKey(KeyCode.LeftArrow))
			GetComponent<Transform> ().position = GetComponent<Transform> ().position - Vector3.right * speed * Time.deltaTime;
	}
}
