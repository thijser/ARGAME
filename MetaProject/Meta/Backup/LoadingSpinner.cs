// Decompiled with JetBrains decompiler
// Type: LoadingSpinner
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

internal class LoadingSpinner : MonoBehaviour
{
  [SerializeField]
  private Texture2D[] spinnerTextures;
  [SerializeField]
  private float timeBetweenFrames;
  private int counter;
  private float timeSinceLastFrame;

  public LoadingSpinner()
  {
    base.\u002Ector();
  }

  private void Start()
  {
  }

  private void Update()
  {
    this.timeSinceLastFrame += Time.get_deltaTime();
    if ((double) this.timeSinceLastFrame <= (double) this.timeBetweenFrames)
      return;
    this.counter = (this.counter + 1) % this.spinnerTextures.Length;
    ((Renderer) ((Component) this).get_gameObject().GetComponent<Renderer>()).get_material().set_mainTexture((Texture) this.spinnerTextures[this.counter]);
    this.timeSinceLastFrame = 0.0f;
  }
}
