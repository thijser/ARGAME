using UnityEngine;
using System.Collections;

public class TargetAnimation : MonoBehaviour {
	private Animation anim;
	public AnimationClip Open;

	public void Start () {
		anim = GetComponent<Animation>();
		anim.AddClip (Open, "Open");
		anim.Play ("Open");
	}
	public void Update() {
		if (Input.GetKeyDown ("space")) {
			Rewind ();
			Debug.Log("reversing");
		}
	}
	public void Rewind() {
		anim.Rewind ();
	}

}
