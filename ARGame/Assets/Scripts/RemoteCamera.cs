﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Camera movement behavior for the remote camera view.
/// </summary>
public class RemoteCamera : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the angle of the camera.
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    /// Gets or sets the altitude of the camera.
    /// </summary>
    public float Altitude { get; set; }

    /// <summary>
    /// Gets or sets the distance of the camera.
    /// </summary>
    public float Distance { get; set; }

    /// <summary>
    /// Gets or sets the horizontal rotation speed.
    /// </summary>
    public float HorizontalAngleSpeed { get; set; }

    /// <summary>
    /// Gets or sets the vertical rotation speed.
    /// </summary>
    public float VerticalAngleSpeed { get; set; }

    /// <summary>
    /// Gets or sets the speed with which the distance changes.
    /// </summary>
    public float DistanceSpeed { get; set; }

    /// <summary>
    /// Initializes the camera to default values.
    /// </summary>
    public void Start()
    {
        this.HorizontalAngleSpeed = 2.5f;
        this.VerticalAngleSpeed = 1.25f;
        this.DistanceSpeed = 0.18f;

        this.ResetCamera();
    }

    /// <summary>
    /// Resets the orientation of the camera to default values.
    /// </summary>
    public void ResetCamera()
    {
        this.Angle = 0;
        this.Altitude = 30;
        this.Distance = 5;
    }

    /// <summary>
    /// Updates the camera corresponding with user input.
    /// </summary>
    public void FixedUpdate()
    {
        this.CheckInput();
        this.UpdateCamera();
    }

    /// <summary>
    /// Checks user input and updates the values in this script accordingly.
    /// </summary>
    public void CheckInput()
    {
        float horizontal = 0;
        float vertical = 0;
        float distance = 0;

        if (Input.GetKey(KeyCode.R))
        {
            this.ResetCamera();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal += this.HorizontalAngleSpeed;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontal -= this.HorizontalAngleSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            vertical += this.VerticalAngleSpeed;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            vertical -= this.VerticalAngleSpeed;
        }

        if (Input.GetKey(KeyCode.Minus))
        {
            distance += this.DistanceSpeed;
        }

        if (Input.GetKey(KeyCode.Equals))
        {
            distance -= this.DistanceSpeed;
        }

        this.Angle = this.Angle + horizontal;
        this.Altitude = Mathf.Clamp(this.Altitude + vertical, 15, 85);
        this.Distance = Mathf.Clamp(this.Distance + distance, 2.3f, float.PositiveInfinity);
    }

    /// <summary>
    /// Updates the camera position and rotation based on the current values.
    /// </summary>
    public void UpdateCamera()
    {
        this.GetComponentInChildren<Camera>().transform.localPosition = new Vector3(0, 0, -this.Distance);
        this.transform.localRotation = Quaternion.Euler(this.Altitude, this.Angle, 0);
    }
}
