using UnityEngine;
using System.Collections;
using System;

namespace Laser
{
  public class Mirror : MonoBehaviour, ILaserReceiver
  {

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
