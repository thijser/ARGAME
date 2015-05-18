using UnityEngine;
using System.IO;
using System.Collections;

public class CubeCams : MonoBehaviour {
    public Camera camLeft;
    public Camera camRight;

    private float xSeparation = 0.0f;
    private float yOffset = 0.0f;
    private float zOffset = 0.0f;
    private float fov = 35.0f;

    private float lastSave = 0.0f;

    void Start() {
        camLeft.aspect = 16f / 9f;
        camRight.aspect = 16f / 9f;
    }

    void Update() {
        float acc = 1.0f;
        if (Input.GetKey(KeyCode.LeftShift)) acc = 10.0f;

        if (Input.GetKey(KeyCode.W)) {
            xSeparation += 0.01f * acc;
        }
        if (Input.GetKey(KeyCode.S)) {
            xSeparation -= 0.01f * acc;
        }
        if (Input.GetKey(KeyCode.E)) {
            zOffset += 0.01f * acc;
        }
        if (Input.GetKey(KeyCode.D)) {
            zOffset -= 0.01f * acc;
        }
        if (Input.GetKey(KeyCode.R)) {
            fov += 0.1f * acc;
        }
        if (Input.GetKey(KeyCode.F)) {
            fov -= 0.1f * acc;
        }
        if (Input.GetKey(KeyCode.T)) {
            yOffset += 0.01f * acc;
        }
        if (Input.GetKey(KeyCode.G)) {
            yOffset -= 0.01f * acc;
        }

        xSeparation = Mathf.Max(0, xSeparation);

        camLeft.transform.position = new Vector3(-xSeparation, yOffset, zOffset);
        camRight.transform.position = new Vector3(xSeparation, yOffset, zOffset);

        camLeft.fieldOfView = fov;
        camRight.fieldOfView = fov;

        if (Time.time - lastSave > 1.0f) {
            File.WriteAllText("C:/Users/Alexander/Desktop/code.txt", xSeparation + "|" + yOffset + "|" + zOffset + "|" + fov);
            lastSave = Time.time;
        }
    }
}
