//----------------------------------------------------------------------------
// <copyright file="Webcam.cs" company="Delft University of Technology">
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
/// Manages images retrieved from a webcam.
/// </summary>
public class Webcam : MonoBehaviour
{
    /// <summary>
    /// The WebcamTexture object to use.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public WebCamTexture WebcamTexture;

    /// <summary>
    /// The name of the webcam to get the image from.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public string WebcamName;

    /// <summary>
    /// Sets up the <c>WebcamTexture</c> to display the image from the webcam with the 
    /// name given by <c>WebcamName</c>.
    /// </summary>
    public void Start()
    {
        foreach (WebCamDevice dev in WebCamTexture.devices)
        {
            Debug.Log(dev.name);
        }

        this.WebcamTexture = new WebCamTexture(this.WebcamName);
        gameObject.GetComponent<Renderer>().material.mainTexture = this.WebcamTexture;
        this.WebcamTexture.Play();

        Debug.Log(this.WebcamTexture.width + ", " + this.WebcamTexture.height);
    }

    /// <summary>
    /// Displays controls for starting or stopping the camera feed.
    /// </summary>
    public void OnGUI()
    {
        if (this.WebcamTexture.isPlaying)
        {
            if (GUILayout.Button("Pause"))
            {
                this.WebcamTexture.Pause();
            }

            if (GUILayout.Button("Stop"))
            {
                this.WebcamTexture.Stop();
            }
        }
        else
        {
            if (GUILayout.Button("Play"))
            {
                this.WebcamTexture.Play();
            }
        }
    }
}
