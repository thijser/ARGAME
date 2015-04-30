using UnityEngine;
using System.Collections;
using System;

namespace Laser
{
  ///<summary>
  ///A Mirror that reflects Laser beams.
  ///</summary>
  public class Mirror : MonoBehaviour, ILaserReceiver
  {
    ///<summary>
    ///Reflects the argument Laser beam and creates a new Laser beam
    ///in the reflected direction.
    ///</summary>
    ///<param name="laser">The Laser beam that hits this Mirror.</param>
    public void OnLaserHit(Laser laser)
    {
      if (laser == null)
      {
        throw new ArgumentNullException();
      }
      Vector3 normal = gameObject.transform.rotation * Vector3.back;
      Vector3 reflection = Vector3.Reflect (laser.direction, normal);
      laser.Extend(laser.endpoint, reflection);
    }
  }

}
