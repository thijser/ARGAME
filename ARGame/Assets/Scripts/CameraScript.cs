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
		float distance=Mathf.Abs((transform.position-board.position).magnitude);
		Debug.Log (distance);
		float toMove=distance-zoom;
		transform.LookAt(board);
		transform.Translate(transform.forward*toMove);
		arrowMove();

		
	}
	public void FreeArrowCam(){
		
	}
	public void FollowPlayer(){

	}
	public void FixedPosition(){
		
	}
}
