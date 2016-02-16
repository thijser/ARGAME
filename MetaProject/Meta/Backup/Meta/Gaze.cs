// Decompiled with JetBrains decompiler
// Type: Meta.Gaze
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class Gaze : MetaSingleton<Gaze>
  {
    private int m_numRows = 5;
    private int m_raysPerRow = 5;
    private float m_angle = 5f;
    public float[] m_rowWeights = new float[5];
    public float dwellTimeThreshold = 2f;
    public LayerMask m_layerMask;
    public bool m_showRays;
    private bool m_descend;
    public GameObject objectOfInterest;
    private bool m_useWeights;
    public GameObject gazeTarget;
    public GameObject lastGazeTarget;
    public bool isGazeOver;
    public float gazeTargetTime;
    public float lastGazeTargetTime;
    public float gazeTimeElapsed;
    public bool hasGazeDwelled;
    public int numHits;

    private void UpdateMultiraycast()
    {
      RaycastHit[] hits = MultiRaycast.MultiRayCast(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_forward(), this.m_numRows, this.m_raysPerRow, this.m_angle, this.m_layerMask, this.m_descend);
      this.objectOfInterest = !this.m_useWeights ? MultiRaycast.MostHit(hits) : MultiRaycast.MostHitWithWeights(hits, this.m_rowWeights);
      if (!this.m_showRays)
        return;
      foreach (RaycastHit raycastHit in hits)
      {
        // ISSUE: explicit reference operation
        Debug.DrawLine(((Component) this).get_transform().get_position(), ((RaycastHit) @raycastHit).get_point());
      }
    }

    private void Start()
    {
    }

    private void Update()
    {
      this.UpdateMultiraycast();
      this.GazeCast();
    }

    public void CheckGazeAssertions()
    {
    }

    public void GazeCast()
    {
      this.CheckGazeAssertions();
      this.isGazeOver = false;
      RaycastHit[] raycastHitArray = Physics.RaycastAll(Camera.get_main().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)), 100000f, -1);
      this.numHits = raycastHitArray != null ? raycastHitArray.Length : 0;
      if (raycastHitArray != null && raycastHitArray.Length > 0)
      {
        foreach (RaycastHit raycastHit in raycastHitArray)
        {
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          if (Object.op_Inequality((Object) ((RaycastHit) @raycastHit).get_collider(), (Object) null) && Object.op_Inequality((Object) ((Component) ((RaycastHit) @raycastHit).get_collider()).get_gameObject().GetComponent<MetaBody>(), (Object) null))
          {
            // ISSUE: explicit reference operation
            MetaBody metaBody = (MetaBody) ((Component) ((RaycastHit) @raycastHit).get_collider()).get_gameObject().GetComponent<MetaBody>();
            if (Object.op_Inequality((Object) this.gazeTarget, (Object) null) && Object.op_Inequality((Object) ((Component) metaBody).get_gameObject(), (Object) this.gazeTarget) && (((MetaBody) this.gazeTarget.GetComponent<MetaBody>()).gazeableDwellable && this.hasGazeDwelled))
            {
              this.gazeTarget.get_gameObject().SendMessage("OnGazeDwellExit", (SendMessageOptions) 1);
              this.hasGazeDwelled = false;
              return;
            }
            if (metaBody.gazeable)
            {
              this.isGazeOver = true;
              if (Object.op_Inequality((Object) ((Component) metaBody).get_gameObject(), (Object) this.gazeTarget))
              {
                if (Object.op_Inequality((Object) this.gazeTarget, (Object) null))
                  this.gazeTarget.SendMessage("OnGazeExit", (SendMessageOptions) 1);
                this.gazeTargetTime = Time.get_time();
                this.gazeTimeElapsed = 0.0f;
                this.lastGazeTargetTime = this.gazeTargetTime;
                ((Component) metaBody).get_gameObject().SendMessage("OnGazeEnter", (SendMessageOptions) 1);
                this.lastGazeTarget = this.gazeTarget;
                this.gazeTarget = ((Component) metaBody).get_gameObject();
              }
              else
              {
                ((Component) metaBody).get_gameObject().SendMessage("OnGazeHold", (SendMessageOptions) 1);
                this.gazeTimeElapsed = Time.get_time() - this.gazeTargetTime;
                if (metaBody.gazeableDwellable && (double) this.gazeTimeElapsed > (double) this.dwellTimeThreshold)
                {
                  if (!this.hasGazeDwelled)
                  {
                    ((Component) metaBody).get_gameObject().SendMessage("OnGazeDwell", (SendMessageOptions) 1);
                    this.hasGazeDwelled = true;
                  }
                  else
                    ((Component) metaBody).get_gameObject().SendMessage("OnGazeDwellHold", (SendMessageOptions) 1);
                }
              }
              this.isGazeOver = true;
              break;
            }
          }
        }
      }
      if (this.isGazeOver)
        return;
      if (Object.op_Inequality((Object) this.gazeTarget, (Object) null) && ((MetaBody) this.gazeTarget.GetComponent<MetaBody>()).gazeable)
      {
        if (((MetaBody) this.gazeTarget.GetComponent<MetaBody>()).gazeableDwellable && this.hasGazeDwelled)
        {
          this.gazeTarget.SendMessage("OnGazeDwellExit", (SendMessageOptions) 1);
          this.hasGazeDwelled = false;
          return;
        }
        this.gazeTarget.SendMessage("OnGazeExit", (SendMessageOptions) 1);
        this.lastGazeTarget = this.gazeTarget;
        this.gazeTarget = (GameObject) null;
        this.hasGazeDwelled = false;
      }
      this.gazeTimeElapsed = 0.0f;
      this.gazeTargetTime = 0.0f;
      this.isGazeOver = false;
      this.lastGazeTarget = this.gazeTarget;
      this.gazeTarget = (GameObject) null;
    }
  }
}
