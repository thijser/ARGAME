using UnityEngine;
using System.Collections;

public class PlayerColour : MonoBehaviour {
	public Color colour;
	void start(){
		setColour(colour);
	}
	public void setColour(Color c){
		Renderer renderer=gameObject.GetComponent<Renderer>();
		renderer.material.color=c;
		renderer.material.SetColor(0,c);
		renderer.material.SetColor(1,c);
		renderer.material.SetColor(2,c);
		renderer.material.SetColor(3,c);
		renderer.material.SetColor(4,c);
		renderer.material.SetColor(5,c);

	}

}
