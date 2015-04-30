using UnityEngine;
using System.Collections;

public class TargetAnimation : MonoBehaviour {
	Animation anim;
	public AnimationClip Open; 
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animation>();
		anim.AddClip (Open, "Open");
		anim.Play ("Open");

	}
}
