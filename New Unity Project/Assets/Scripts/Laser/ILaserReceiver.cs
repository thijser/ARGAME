using UnityEngine;
using System.Collections;

namespace Laser
{
  public interface ILaserReceiver
  {
    void OnLaserHit(Laser laser);
  }

}
