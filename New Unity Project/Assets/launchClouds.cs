using UnityEngine;
using System.Collections;

public class launchClouds : MonoBehaviour {
	
	public ComputeShader shader;
	public RenderTexture output; 
	void Start(){
		output.enableRandomWrite = true;
		output.Create();
		
	}
	void Update(){
		RunShader();
	}
	void RunShader()
	{
		int kernelHandle = shader.FindKernel("CSMain");
		shader.SetTexture(kernelHandle, "Result", output);
		shader.Dispatch(kernelHandle, 256/8, 256/8, 1);
	}
}
