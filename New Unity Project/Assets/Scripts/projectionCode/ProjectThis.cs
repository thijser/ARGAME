using UnityEngine;
using System.Collections;

public class ProjectThis : MonoBehaviour {
	public projectioncode Projector; 

	
	// Update is called once per frame
	void Update () {
		Projector.projectTransform(transform);
	}
}
