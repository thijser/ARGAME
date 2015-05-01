using UnityEngine;
using System.Collections;
using System;

namespace Laser {
    ///<summary>
    ///A Mirror that reflects Laser beams.
    ///</summary>
    public class Mirror : MonoBehaviour, ILaserReceiver {
        ///<summary>
        ///Reflects the argument Laser beam and creates a new Laser beam
        ///in the reflected direction.
        ///</summary>
        ///<param name="laser">The Laser beam that hits this Mirror.</param>
        public void OnLaserHit(object sender, HitEventArgs args) {
            if (args == null) {
                throw new ArgumentNullException();
            }
            Vector3 reflection = Vector3.Reflect(args.laser.direction, args.normal);
            args.laser.Extend(args.laser.endpoint, reflection);
        }
    }

}
