using System;
using UnityEngine;

namespace Laser {

    ///<summary>
    ///Describes the event of a Laser beam hitting an object.
    ///</summary>
    public class HitEventArgs : EventArgs {

        ///<summary>
        ///The Laser beam that hit the object.
        ///</summary>
        public Laser laser { get; set; }
        ///<summary>
        ///The normal of the surface that the Laser beam hit.
        ///</summary>
        public Vector3 normal { get; set; }

        ///<summary>
        ///Creates a new HitEventArgs instance with the given arguments.
        ///</summary>
        ///<param name="laser">The Laser beam that hit the object</param>
        ///<param name="normal">The normal of the surface that the Laser beam hit</param>
        public HitEventArgs(Laser laser, Vector3 normal) {
          this.laser = laser;
          this.normal = normal;
        }
    }
}
