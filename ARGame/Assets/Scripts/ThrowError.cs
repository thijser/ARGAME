using UnityEngine;
using System.Collections;
using System;
public class ThrowError : MonoBehaviour {

	// Use this for initialization
	void Start () {
		throw new InvalidOperationException();
	}
	
	// Update is called once per frame
	void Update () {
		throw new InvalidOperationException();	
	}
}
