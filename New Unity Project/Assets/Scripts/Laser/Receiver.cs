using UnityEngine;
using System.Collections;

namespace Laser {

public class Receiver : MonoBehaviour, ILaserReceiver {

	public ILaserReceiver laserBehavior;

	public void OnLaserHit(Laser laser) {
		laserBehavior.OnLaserHit (laser);
	}
}

}