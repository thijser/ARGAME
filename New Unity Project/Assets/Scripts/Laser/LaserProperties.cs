using UnityEngine;
using System.Collections;

public class LaserProperties : MonoBehaviour {
	public Vector3 RGBStrengths=new Vector3(1,1,1);
	 LineRenderer lineRenderer;

	void Start() {
		lineRenderer = gameObject.GetComponent<LineRenderer>();
	}

	/*
	 * returns the color in RGB.
	 */
	Color getColor(){
		Color c = new Color (0, 0, 0, 1);
		float highest = Mathf.Max (RGBStrengths.x, RGBStrengths.y, RGBStrengths.z);
		c.r = RGBStrengths.x / highest;
		c.g = RGBStrengths.y / highest;
		c.b = RGBStrengths.z / highest;
		return c;
	}
	/*
	 * returns the total amount of energy in the beam 
	 */
	float getStrength(){
		return RGBStrengths.magnitude;
	}
	void Update(){
		updateBeam ();
	}
	/*
	 * set color of the beam to the color of the laser 
	 */
	public void updateBeam(){
		lineRenderer.SetWidth (getStrength (), getStrength());
		lineRenderer.material.color = getColor ();
		lineRenderer.material.SetColor ("_Albedo", getColor ());
		lineRenderer.material.SetColor ("_Emission", getColor ());
		lineRenderer.material.SetColor ("Main Color", getColor ());
	}
}
