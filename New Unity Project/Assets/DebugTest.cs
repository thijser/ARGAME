using UnityEngine;
using System.Collections;

public class DebugTest : MonoBehaviour {
	public Transform level;
	public Transform mirror;

	Vector3 originalPos, newPos;

	// Use this for initialization
	void Start () {
		originalPos = mirror.position;

		Vector3 v1 = mirror.position - level.position;
		Vector3 n = level.up;

		Vector3 proj = v1 - Vector3.Dot (v1, n) * n;
		proj += level.position;

		//mirror.position = proj;
		newPos = proj;
	}
	
	// Update is called once per frame
	void Update () {
		mirror.position = Vector3.Lerp (originalPos, newPos, Time.fixedTime - Mathf.Floor (Time.fixedTime));
	}
}
