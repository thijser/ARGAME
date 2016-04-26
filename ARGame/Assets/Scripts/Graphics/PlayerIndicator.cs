using UnityEngine;
using System.Collections;

public class PlayerIndicator : MonoBehaviour {
	void Start () {
        // Clone material such that each indicator is colored individually
        GetComponent<MeshRenderer>().material = new Material(GetComponent<MeshRenderer>().material);
	}
}
