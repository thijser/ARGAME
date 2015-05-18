//----------------------------------------------------------------------------
// <copyright file="CubeCams.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

/// <summary>
/// Allows customization of camera offset information.
/// </summary>
public class CubeCams : MonoBehaviour
{
    /// <summary>
    /// The Left-Eye Camera.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public Camera LeftCamera;

    /// <summary>
    /// The Right-Eye Camera.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public Camera RightCamera;

    /// <summary>
    /// The horizontal separation between the left and right cameras.
    /// </summary>
    private float xSeparation = 0.0f;

    /// <summary>
    /// The offset in the y-axis.
    /// </summary>
    private float yOffset = 0.0f;

    /// <summary>
    /// The offset in the z-axis.
    /// </summary>
    private float zOffset = 0.0f;

    /// <summary>
    /// The field-of-vision of the Cameras.
    /// </summary>
    private float fov = 35.0f;

    /// <summary>
    /// The time the configuration was last saved to disk.
    /// </summary>
    private float lastSave = 0.0f;

    /// <summary>
    /// Initializes the aspect ratio of the Cameras.
    /// </summary>
    public void Start()
    {
        this.LeftCamera.aspect = 16f / 9f;
        this.RightCamera.aspect = 16f / 9f;
    }

    /// <summary>
    /// Updates the configuration of the Cameras when the corresponding keys are pressed.
    /// </summary>
    public void Update()
    {
        float acc = 1.0f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            acc = 10.0f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.xSeparation += 0.01f * acc;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.xSeparation -= 0.01f * acc;
        }

        if (Input.GetKey(KeyCode.E))
        {
            this.zOffset += 0.01f * acc;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.zOffset -= 0.01f * acc;
        }

        if (Input.GetKey(KeyCode.R))
        {
            this.fov += 0.1f * acc;
        }

        if (Input.GetKey(KeyCode.F))
        {
            this.fov -= 0.1f * acc;
        }

        if (Input.GetKey(KeyCode.T))
        {
            this.yOffset += 0.01f * acc;
        }

        if (Input.GetKey(KeyCode.G))
        {
            this.yOffset -= 0.01f * acc;
        }

        this.xSeparation = Mathf.Max(0, this.xSeparation);

        this.LeftCamera.transform.position = new Vector3(-this.xSeparation, this.yOffset, this.zOffset);
        this.RightCamera.transform.position = new Vector3(this.xSeparation, this.yOffset, this.zOffset);

        this.LeftCamera.fieldOfView = this.fov;
        this.RightCamera.fieldOfView = this.fov;

        if (Time.time - this.lastSave > 1.0f)
        {
            File.WriteAllText("C:/Users/Alexander/Desktop/code.txt", this.xSeparation + "|" + this.yOffset + "|" + this.zOffset + "|" + this.fov);
            this.lastSave = Time.time;
        }
    }
}
