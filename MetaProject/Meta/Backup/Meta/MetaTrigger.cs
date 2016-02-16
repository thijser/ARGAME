// Decompiled with JetBrains decompiler
// Type: Meta.MetaTrigger
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class MetaTrigger : MonoBehaviour
  {
    public MetaTrigger.Mode action;
    public string triggerName;
    public string messageName;
    public string messageValue;
    public Object target;
    public GameObject source;
    public int triggerCount;
    public bool repeatTrigger;
    public bool activateOnClick;
    public bool activateOnFinger;
    public bool deactivateToggle;
    public bool delayingTimer;
    public bool autoStartAfterDelay;
    public float delayTime;
    public float delayTimeStart;
    public bool activateOnReticle;
    public float activateOnReticleDelay;
    public bool activateOnReticleFeedback;
    public bool delayedTrigger;
    public bool delaying;
    public string url;

    public MetaTrigger()
    {
      base.\u002Ector();
    }

    public void DoTrigger()
    {
      if (this.delayedTrigger)
        this.delaying = true;
      else
        this.HandleTrigger();
    }

    private void HandleTrigger()
    {
      // ISSUE: unable to decompile the method.
    }

    public void StartDelay()
    {
      this.delayTimeStart = Time.get_time();
      this.delayingTimer = true;
    }

    public void OnReticleHit()
    {
      if (!this.activateOnReticle)
        return;
      this.DoTrigger();
    }

    private void Start()
    {
      if (!this.autoStartAfterDelay)
        return;
      this.StartDelay();
    }

    private void Update()
    {
      if (this.activateOnFinger)
      {
        if (Hands.left.isValid)
        {
          Bounds bounds = ((Collider) ((Component) this).GetComponent<Collider>()).get_bounds();
          // ISSUE: explicit reference operation
          if (((Bounds) @bounds).Contains(Hands.left.pointer.position))
            goto label_5;
        }
        if (Hands.right.isValid)
        {
          Bounds bounds = ((Collider) ((Component) this).GetComponent<Collider>()).get_bounds();
          // ISSUE: explicit reference operation
          if (!((Bounds) @bounds).Contains(Hands.right.pointer.position))
            goto label_6;
        }
        else
          goto label_6;
label_5:
        this.DoTrigger();
      }
label_6:
      if (!this.delayingTimer || (double) Time.get_time() - (double) this.delayTimeStart <= (double) this.delayTime)
        return;
      this.DoTrigger();
    }

    private void LateUpdate()
    {
      if (!this.delayedTrigger || !this.delaying)
        return;
      this.HandleTrigger();
      this.delaying = false;
    }

    private void OnMouseUp()
    {
      if (!this.activateOnClick)
        return;
      this.DoTrigger();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (this.activateOnFinger)
      {
        if (!((Object) other).get_name().Contains("Pointer"))
          return;
        this.DoTrigger();
      }
      else
        this.DoTrigger();
    }

    public enum Mode
    {
      Trigger,
      Replace,
      Activate,
      Enable,
      Animate,
      Deactivate,
      Message,
      URL,
    }
  }
}
