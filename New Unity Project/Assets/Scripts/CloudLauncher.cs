//----------------------------------------------------------------------------
// <copyright file="CloudLauncher.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Runs the CloudRenderer <c>ComputeShader</c>.
/// </summary>
public class CloudLauncher : MonoBehaviour
{
    /// <summary>
    /// The <c>ComputeShader</c> to run.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public ComputeShader Shader;

    /// <summary>
    /// The output RenderTexture.
    /// </summary>
    private RenderTexture output;

    /// <summary>
    /// Initializes the output RenderTexture.
    /// </summary>
    public void Start()
    {
        this.output = new RenderTexture(256, 256, 24);
        this.output.enableRandomWrite = true;
    }

    /// <summary>
    /// Runs the <c>ComputeShader</c>.
    /// </summary>
    public void Update()
    {
        this.RunShader();
    }

    /// <summary>
    /// Runs the <c>ComputeShader</c>.
    /// </summary>
    public void RunShader()
    {
        int kernelHandle = this.Shader.FindKernel("CSMain");
        this.output.Create();
        Material mat = gameObject.GetComponent<Skybox>().material;
        mat.SetTexture("_FrontTex", this.output);
        mat.SetTexture("_BackTex", this.output);
        mat.SetTexture("_LeftTex", this.output);
        mat.SetTexture("_RightTex", this.output);
        mat.SetTexture("_UpTex", this.output);
        mat.SetTexture("_DownTex", this.output);
        this.Shader.SetTexture(kernelHandle, "Result", this.output);
        this.Shader.Dispatch(kernelHandle, 32, 32, 1);
    }
}
