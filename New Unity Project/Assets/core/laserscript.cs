using UnityEngine;
using System.Collections;

public class laserscript : MonoBehaviour {
	public Transform start; 
	public bool first=true;
	public ParticleSystem me;
	private int maskSolids=1+2;
	private int maskLauncherAndSolids=1+2+512;
	RaycastHit gethit(){
		int layermask;
		if (first) {
			layermask = maskSolids;
		}else{
			layermask=maskLauncherAndSolids;
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(start.position,start.forward,out hitInfo,Mathf.Infinity,layermask)) {
			Debug.Log(" hit");
			return hitInfo;
		}
		return hitInfo;
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		gethit ();
	}
}
