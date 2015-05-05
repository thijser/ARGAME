using System.Collections;
﻿using UnityEngine;

/// <summary>
/// Provides an Animation for the Laser Target.
/// </summary>
public class TargetAnimation : MonoBehaviour
{
    /// <summary>
    /// The Animation for the Laser Target.
    /// </summary>
	private Animation anim;

    /// <summary>
    /// The AnimationClip for the LaserTarget.
    /// </summary>
	public AnimationClip OpenClip;

    /// <summary>
    /// Starts the Animation.
    /// </summary>
	public void Start()
    {
		this.anim = GetComponent<Animation>();
		this.anim.AddClip(this.OpenClip, "Open");
        this.anim.Play("Open");
    }

    /// <summary>
    /// Rewinds the Animation if (and only if) the space bar is pressed.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            this.Rewind();
            Debug.Log("reversing");
        }
    }

    /// <summary>
    /// Rewinds the Animation.
    /// </summary>
    public void Rewind()
    {
        this.anim.Rewind();
    }
}
