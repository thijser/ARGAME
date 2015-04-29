using UnityEngine;
using System.Collections;

namespace Laser
{
  public class Receiver : MonoBehaviour, ILaserReceiver
  {

    public MonoBehaviour laserBehaviour;

    public void OnLaserHit(Laser laser)
    {
      ILaserReceiver receiver = laserBehaviour as ILaserReceiver;
      if (receiver != null)
        {
          receiver.OnLaserHit(laser);
        } else
          {
            Debug.LogError("LaserBehaviour set to a non-ILaserReceiver script");
          }
    }
  }

    }
