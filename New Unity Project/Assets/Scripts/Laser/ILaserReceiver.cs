using UnityEngine;
using System.Collections;

namespace Laser
{
  ///<summary>
  ///An object that is manipulated by a Laser beam.
  ///</summary>
  ///<seealso cref="Laser.Laser" />
  public interface ILaserReceiver
  {
    ///<summary>
    ///Called every time the object is hit by a laser beam.
    ///</summary>
    ///<param name="laser">The Laser object that hits this object</param>
    void OnLaserHit(Laser laser);
  }

}
