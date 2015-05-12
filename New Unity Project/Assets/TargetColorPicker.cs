using UnityEngine;
using System.Collections;

public class TargetColorPicker : MonoBehaviour {

	public Color color;
	// Use this for initialization
	void Update () {
		Renderer ren = gameObject.GetComponent<Renderer>();
		ren.material.SetColor("Color",color);
	}

}
