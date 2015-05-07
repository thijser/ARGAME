using UnityEngine;
using System.Collections;

public class printLocation : MonoBehaviour 
{
	public void Update () {
		Debug.Log ("position:"  + transform.position);
		Debug.Log ("rotation:" + transform.rotation);
	}
}
