using UnityEngine;
using System.Collections;

public class ImageLinker : MonoBehaviour {
	public projectioncode Projector; 
	public Transform linkedTo; 
	void Start(){

		Projector.addPosition (transform);
	}
	
	// Update is called once per frame
	void Update () {
		Projector.projectTransform(linkedTo,transform);
		Projector.rotate (linkedTo);
	}
}
