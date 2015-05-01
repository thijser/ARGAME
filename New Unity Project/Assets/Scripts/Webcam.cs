using UnityEngine;

public class Webcam : MonoBehaviour
{
  public WebCamTexture webcamTexture;
  public string webcamName;

  public void Start()
  {
    foreach (WebCamDevice dev in WebCamTexture.devices)
    {
      Debug.Log(dev.name);
    }

    webcamTexture = new WebCamTexture(webcamName);
    gameObject.GetComponent<Renderer>().material.mainTexture = webcamTexture;
    webcamTexture.Play();

    Debug.Log(webcamTexture.width + ", " + webcamTexture.height);
  }

  public void OnGUI()
  {
    if (webcamTexture.isPlaying)
    {
      if (GUILayout.Button("Pause"))
      {
        webcamTexture.Pause();
      }
      if (GUILayout.Button("Stop"))
      {
        webcamTexture.Stop();
      }
    }
    else
    {
      if (GUILayout.Button("Play"))
      {
        webcamTexture.Play();
      }
    }
  }
}
