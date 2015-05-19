using UnityEngine;
using System.Collections;

public class launchClouds : MonoBehaviour {

	public ComputeShader shader;
	private RenderTexture output; 
	void Start(){

	}
	void Update(){
		RunShader();
	}


	void RunShader()
	{
		int kernelHandle = shader.FindKernel("CSMain");
		RenderTexture output = new RenderTexture(256,256,24);
		output.enableRandomWrite = true;
		output.Create();
		Material mat = gameObject.GetComponent<Skybox>().material;
		mat.SetTexture("_FrontTex",output);
		mat.SetTexture("_BackTex",output);
		mat.SetTexture("_LeftTex",output);
		mat.SetTexture("_RightTex",output);
		mat.SetTexture("_UpTex",output);
		mat.SetTexture("_DownTex",output);
		shader.SetTexture(kernelHandle,
		                  "Result", output);
		shader.Dispatch(kernelHandle, 256/8, 256/8, 1);
	}
}
