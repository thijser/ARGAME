using UnityEngine;
using System.Collections;

namespace Laser {
    ///<summary>
    ///Dynamic Receiver ready to be used in the Unity Editor.
    ///Because the Unity editor can only deal with specific types
    ///and not with interfaces, this class enables registration of
    ///ILaserReceiver instances from within the Unity Editor.
    ///
    ///To use this class, add it to a gameObject in the Editor.
    ///Also add the class you want to use as a script.
    ///Then drag the script of the class you want to use into
    ///the "Laser Behaviour" field in the Unity Editor.
    ///This will cause the desired behaviour to be invoked when
    ///the object is hit with a Laser beam.
    ///</summary>
    public class Receiver : MonoBehaviour, ILaserReceiver {

        ///<summary>
        ///The ILaserReceiver to call when this object is hit by a Laser beam.
        ///</summary>
        public MonoBehaviour laserBehaviour;

        ///<summary>
        ///Calls the delegate laserBehaviour object if it is a valid
        ///ILaserReceiver instance. Logs an error message otherwise.
        ///</summary>
        ///<param name="sender">The sender of the event</param>
        ///<param name="args">The HitEventArgs describing the event</param>
        public void OnLaserHit(object sender, HitEventArgs args) {
            ILaserReceiver receiver = laserBehaviour as ILaserReceiver;
            if (receiver != null) {
                receiver.OnLaserHit(this, args);
            } else {
                Debug.LogError("LaserBehaviour set to a non-ILaserReceiver script");
            }
        }
    }
}
