using UnityEngine;
using System.Collections;

public class ProjectThis : MonoBehaviour {
	public projectioncode Projector; 
	void Start(){
		Projector.addPosition (transform);
	}
	
	// Update is called once per frame
	void Update () {
		Projector.projectTransform(transform);
		Projector.rotate (transform);
	}
}
