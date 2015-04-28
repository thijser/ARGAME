using UnityEngine;

public class Webcam : MonoBehaviour {
    //FIXME: Remove commented out code
    //public MeshRenderer[] UseWebcamTexture;
    public WebCamTexture webcamTexture;

    public string webcamName;

    void Start() {
        foreach (WebCamDevice dev in WebCamTexture.devices) {
            Debug.Log(dev.name);
        }

        webcamTexture = new WebCamTexture(webcamName);
        //FIXME: Remove commented out code
        /*foreach (MeshRenderer r in UseWebcamTexture) {
            r.material.mainTexture = webcamTexture;
        }*/
        //renderer.material.mainTexture = webcamTexture;
        gameObject.GetComponent<Renderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();

        Debug.Log(webcamTexture.width + ", " + webcamTexture.height);
    }

    void OnGUI() {
        if (webcamTexture.isPlaying) {
            if (GUILayout.Button("Pause")) {
                webcamTexture.Pause();
            }
            if (GUILayout.Button("Stop")) {
                webcamTexture.Stop();
            }
        } else {
            if (GUILayout.Button("Play")) {
                webcamTexture.Play();
            }
        }
    }
}
