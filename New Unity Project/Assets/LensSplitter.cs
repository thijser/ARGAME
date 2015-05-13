using UnityEngine;
using System.Collections;
using Core.Emitter;
using Core;
using System;
using Core.Receiver;

public class LensSplitter : MonoBehaviour, ILaserReceiver {
    /// <summary>
    /// Transform of crystal part.
    /// </summary>
    private Transform focusPoint;

    /// <summary>
    /// GameObject of the left output part.
    /// </summary>
    private GameObject outLeft;

    /// <summary>
    /// GameObject of the right output part.
    /// </summary>
    private GameObject outRight;

    /// <summary>
    /// Set to true if OnLaserHit is called during the frame (before LateUpdate).
    /// </summary>
    private bool hit = false;

    /// <summary>
    /// RGB strengths of incoming Laser beam.
    /// </summary>
    private Vector3 rgbStrengths;

    /// <summary>
    /// Find references to the LensSplitter parts during initialization.
    /// </summary>
    public void Start() {
        focusPoint = transform.parent.Find("focusPoint");
        outLeft = transform.parent.Find("outLeft").gameObject;
        outRight = transform.parent.Find("outRight").gameObject;
    }

    /// <summary>
    /// Curves the incoming laser beam towards the crystal.
    /// </summary>
    /// <param name="sender">The sender of the event, ignored here.</param>
    /// <param name="args">The EventArgs object that describes the event.</param>
    public void OnLaserHit(object sender, HitEventArgs args) {
        if (args == null) {
            throw new ArgumentNullException("args");
        }

        hit = true;
        rgbStrengths = args.Laser.Emitter.GetComponent<LaserProperties>().RGBStrengths;

        // Slight offset to make sure the laser passes through the surface
        Vector3 dir = focusPoint.position - args.Point;
        args.Laser.Extend(args.Point + dir.normalized * 0.1f, dir);
    }

    public void LateUpdate() {
        outLeft.SetActive(hit);
        outRight.SetActive(hit);

        // Split the incoming light between the two outputs
        outLeft.GetComponent<LaserProperties>().RGBStrengths = rgbStrengths / 2;
        outRight.GetComponent<LaserProperties>().RGBStrengths = rgbStrengths / 2;

        hit = false;
    }
}
