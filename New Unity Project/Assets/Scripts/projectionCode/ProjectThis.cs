using UnityEngine;
using System.Collections;

public class ProjectThis : MonoBehaviour {
	projectioncode Projector; 

	
	// Update is called once per frame
	void Update () {
		transform=Projector.projectTransform(transform);
	}
}
