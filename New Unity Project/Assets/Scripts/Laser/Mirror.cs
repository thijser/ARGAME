using UnityEngine;
using System.Collections;

namespace Laser
{
  public class Mirror : MonoBehaviour, ILaserReceiver
  {

    public void OnLaserHit(Laser laser)
    {
      Quaternion normal = gameObject.transform.rotation;
      normal.w = 0;
      Quaternion reflection = normal * laser.direction * normal;
      laser.Extend(laser.endpoint, reflection);
    }
  }

}
