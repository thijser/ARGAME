using System;
using UnityEngine;

internal class LoadingSpinner : MonoBehaviour
{
	[SerializeField]
	private Texture2D[] spinnerTextures;

	[SerializeField]
	private float timeBetweenFrames = 0.2f;

	private int counter;

	private float timeSinceLastFrame;

	private void Start()
	{
	}

	private void Update()
	{
		this.timeSinceLastFrame += Time.get_deltaTime();
		if (this.timeSinceLastFrame > this.timeBetweenFrames)
		{
			this.counter = (this.counter + 1) % this.spinnerTextures.Length;
			base.get_gameObject().GetComponent<Renderer>().get_material().set_mainTexture(this.spinnerTextures[this.counter]);
			this.timeSinceLastFrame = 0f;
		}
	}
}
