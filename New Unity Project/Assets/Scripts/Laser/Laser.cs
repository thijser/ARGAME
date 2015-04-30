using UnityEngine;
using System.Collections;

namespace Laser
{
  public class Laser
  {

    public const float MaxLaserLength = 1000.0f;
    public const float MaxRaycastDist = 20.0f;
    public const string LaserReceiveTag = "LaserReceiver";

    public Vector3 origin { get; set; }

    public Vector3 direction { get; set; }

    public Vector3 endpoint { get; set; }

    private LaserEmitter emitter;

    public Laser(Vector3 origin, Vector3 direction, LaserEmitter emitter)
    {
      this.origin = origin;
      this.direction = direction;
      this.emitter = emitter;
    }

    public void Create()
    {
      RaycastHit hit;
      if (Physics.Raycast (origin, direction, out hit, MaxRaycastDist)) {
        this.endpoint = hit.point;
        emitter.AddLaser (this);
        Collider target = hit.collider;
        Receiver receiver = target.GetComponent<Receiver> ();
        if (receiver != null) {
          receiver.OnLaserHit (this);
        }
      } else {
        this.endpoint = this.origin + (this.direction) * MaxLaserLength;
        emitter.AddLaser (this);
      }
    }

    public Laser Extend(Vector3 newOrigin, Vector3 newDirection)
    {
      Laser l = new Laser(newOrigin, newDirection, emitter);
      l.Create();
      return l;
    }
  }
}
