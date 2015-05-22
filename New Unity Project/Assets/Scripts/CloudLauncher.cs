using UnityEngine;

public class CloudLauncher : MonoBehaviour 
{
	public ComputeShader Shader;

	private RenderTexture output; 

	public void Update()
	{
		RunShader();
	}

	public void RunShader()
	{
		int kernelHandle = Shader.FindKernel("CSMain");
		RenderTexture output = new RenderTexture(256,256,24);
		output.enableRandomWrite = true;
		output.Create();
		Material mat = gameObject.GetComponent<Skybox>().material;
		mat.SetTexture("_FrontTex", output);
		mat.SetTexture("_BackTex", output);
		mat.SetTexture("_LeftTex", output);
		mat.SetTexture("_RightTex", output);
		mat.SetTexture("_UpTex", output);
		mat.SetTexture("_DownTex", output);
		Shader.SetTexture(kernelHandle, "Result", output);
		Shader.Dispatch(kernelHandle, 32, 32, 1);
	}
}
