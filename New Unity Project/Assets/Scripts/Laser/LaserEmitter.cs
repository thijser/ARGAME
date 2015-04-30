using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Laser
{
  public class LaserEmitter : MonoBehaviour
  {

    public static readonly Vector3 Offset = new Vector3(0, 0, 0);
    public const float RenderOffset = 0;
    public LineRenderer lineRenderer;
    private List<Laser> segments = new List<Laser>();

    public void Update()
    {
      Clear();
      MakeLaser();
      Render();
    }

    public void MakeLaser()
    {
      Vector3 pos = gameObject.transform.position + Offset;
      Quaternion dir = gameObject.transform.rotation;
      Laser l = new Laser(pos, dir * Vector3.back, this);
      l.Create();
    }

    public void Clear()
    {
      segments.Clear();
    }

    public void Render()
    {
      lineRenderer.SetVertexCount(segments.Count + 1);
      Vector3 renderOrigin = segments [0].origin + gameObject.transform.forward * RenderOffset;
      lineRenderer.SetPosition (0, renderOrigin);
      for (int i=0; i<segments.Count; i++)
      {
        lineRenderer.SetPosition(i + 1, segments [i].endpoint);
      }
    }

    public void AddLaser(Laser laser)
    {
      segments.Add(laser);
    }
  }
}
