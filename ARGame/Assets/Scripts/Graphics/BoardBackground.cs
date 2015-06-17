using UnityEngine;
using System.Collections;

public class BoardBackground : MonoBehaviour {
	public string ipAdress;
	public string port;
	private Texture texture;
	private bool done;
	public bool UseRemote; 
	WWW www;

	/// <summary>
	/// Start the downloading of the image if UseRemote 
	/// </summary>
	void Start(){
		if(UseRemote){
			if(ipAdress!=""&&ipAdress!=null)
			grabImage();}
	}
	/// <summary>
	/// If we are using the remote check if image is done and use. 
	/// </summary>
	void Update(){
		if(UseRemote){
			tryImage();
		}
	}
	public void onSocketStart(string adres){
		ipAdress=adres;
	}
	/// <summary>
	/// attempts to use the image if it is done loading yet if not we can always try again later 
	/// </summary>
	void tryImage(){
		if(www!=null&&this.www.isDone){
			Renderer renderer = gameObject.GetComponent<Renderer>();
			renderer.material.mainTexture = www.texture;
			Debug.Log ("texture has loaded");
			www.Dispose();
			www=null;
		}

	}

	/// <summary>
	/// start loading the image, note that if the result is to be used then tryImage has to be called later
	/// </summary>
	public void grabImage(){
		Debug.Log ("loading image from: "+ipAdress+":"+port);
		 www = new WWW(ipAdress+":"+port);
	}
}
