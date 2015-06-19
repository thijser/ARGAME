using UnityEngine;
using System.Collections;
public enum Cameratype{
	FreeArrowcamMode=0,
	CentreRotateMode=1,
	FollowPlayerMode=2,
	FixedPositionMode=3
}
public class CameraScript : MonoBehaviour {

	public Cameratype cameratype;
	public bool AllowZoom;
	public float zoom;
	public float Speed;
	Transform board;
	public float AutoSpeed = 0.1f;
	void Start(){
		board=GameObject.FindGameObjectsWithTag("PlayingBoard")[0].transform;
	}
	// Update is called once per frame
	void Update () {
		switch (cameratype){
		case Cameratype.FreeArrowcamMode:
			FreeArrowCam();
			return;
		case Cameratype.CentreRotateMode:
			CentreRotate();
			return;
		case Cameratype.FollowPlayerMode:
			FollowPlayer();
			return;
		case Cameratype.FixedPositionMode:
			FreeArrowCam();
			return;
		}
	}
	public void FollowPlayer(){
	}

	public void arrowMove(){
		if(Input.GetKey(KeyCode.RightArrow)){

			transform.Translate(transform.right*Speed*Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			transform.Translate(transform.up*Speed*Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.LeftArrow)){
			transform.Translate(transform.right*-Speed*Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			transform.Translate(transform.up*-Speed*Time.deltaTime);
		}
	}
	public void CentreRotate(){
		SmoothLookAt(board);
		SmoothmoveToTarget(board,zoom,AutoSpeed);
		arrowMove();
	}

	public void SmoothmoveToTarget(Transform target,float Desireddistance,float speed){
		Vector3 dist=target.position-transform.position;
		float spc=dist.magnitude-Desireddistance;
		Debug.Log (spc);
		transform.Translate(dist*spc*speed);

	}
	public Quaternion SmoothLookAt(Transform target){
		Vector3 dir = transform.position-target.position;
		Quaternion qdir= Quaternion.Euler(dir);
		return Quaternion.Slerp(transform.rotation,qdir,Time.deltaTime*AutoSpeed);
	}

	public void FreeArrowCam(){
		arrowMove();

	}

	public void FixedPosition(){
			
	}
}
