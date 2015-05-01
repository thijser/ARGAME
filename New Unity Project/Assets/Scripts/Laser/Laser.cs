using UnityEngine;
using System.Collections;

namespace Laser {
    ///<summary>
    ///A Laser beam that is emitted into the world.
    ///</summary>
    public class Laser {
        ///<summary>
        ///The maximum distance a laser beam travels.
        ///</summary>
        public const float MaxLaserLength = 1000.0f;
        ///<summary>
        ///The maximum distance the laser beam will check for collisions.
        ///<summary>
        public const float MaxRaycastDist = 20.0f;

        ///<summary>
        ///The origin point of this Laser beam.
        ///<summary>
        public Vector3 origin { get; set; }

        ///<summary>
        ///The direction of this Laser beam.
        ///</summary>
        public Vector3 direction { get; set; }

        ///<summary>
        ///The endpoint of this Laser beam.
        ///This value is available only after this Laser's <c>Create()</c>
        ///is invoked.
        ///</summary>
        public Vector3 endpoint { get; set; }

        private LaserEmitter emitter;

        ///<summary>
        ///Creates a new Laser beam.
        ///</summary>
        ///<param name="origin">The origin of this Laser beam.</param>
        ///<param name="direction">The direction vector of this Laser beam.</param>
        ///<param name="emitter">The LaserEmitter that caused this Laser beam.</param>
        public Laser(Vector3 origin, Vector3 direction, LaserEmitter emitter) {
            this.origin = origin;
            this.direction = direction;
            this.emitter = emitter;
        }

        ///<summary>
        ///Determines the end point of this Laser beam.
        ///The Laser beam can only be drawn after this method is invoked.
        ///Also, until this method is invoked, the value of the <c>endpoint</c>
        ///parameter is undefined.
        ///</summary>
        public void Create() {
            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, MaxRaycastDist)) {
                this.endpoint = hit.point;
                emitter.AddLaser(this);
                Collider target = hit.collider;
                Receiver receiver = target.GetComponent<Receiver>();
                if (receiver != null) {
                    HitEventArgs args = new HitEventArgs(this, hit.normal);
                    receiver.OnLaserHit(this, args);
                }
                // If the Receiver is null, the gameObject simply blocks the laser beam.
            } else {
                this.endpoint = this.origin + (this.direction) * MaxLaserLength;
                emitter.AddLaser(this);
            }
        }

        ///<summary>
        ///Extends this Laser beam in the specified direction.
        ///The returned Laser is already initialized. That is,
        ///its <c>Create()</c> method is invoked.
        ///</summary>
        ///<param name="newOrigin">The origin of the new Laser beam.</param>
        ///<param name="newDirection">The direction of the new Laser beam.</param>
        public Laser Extend(Vector3 newOrigin, Vector3 newDirection) {
            Laser l = new Laser(newOrigin, newDirection, emitter);
            l.Create();
            return l;
        }
    }
}
