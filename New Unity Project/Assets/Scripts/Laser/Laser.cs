using UnityEngine;
using System.Collections;

namespace Laser {

    public class Laser {

      public const float MaxLaserLength = -1000.0f;
      public const float MaxRaycastDist = 20.0f;
      public const string LaserReceiveTag = "LaserReceiver";

      public Vector3 origin { get; set; }
      public Quaternion direction { get; set; }
      public Vector3 endpoint { get; set; }

      private LaserEmitter emitter;

      public Laser (Vector3 origin, Quaternion direction, LaserEmitter emitter) {
        this.origin = origin;
        this.direction = direction;
        this.emitter = emitter;
      }

      public void Create() {
        RaycastHit hit;
        if (Physics.Raycast (origin, direction.eulerAngles, out hit, MaxRaycastDist)) {
          this.endpoint = hit.point;
          Collider target = hit.collider;
          if (target.tag == LaserReceiveTag) {
            target.GetComponent<Receiver> ().OnLaserHit (this);
          }
        }
        else
        {
          this.endpoint = this.origin + (this.direction * Vector3.forward) * MaxLaserLength;
        }
        emitter.AddLaser (this);
      }

      public Laser Extend(Vector3 position, Quaternion direction) {
        Laser l = new Laser (position, direction, emitter);
        l.Create ();
        return l;
      }
    }

}
