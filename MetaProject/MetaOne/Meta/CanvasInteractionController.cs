using System;
using UnityEngine;

namespace Meta
{
	internal class CanvasInteractionController : MonoBehaviour
	{
		private enum LockingMode
		{
			PreLock,
			PostLock
		}

		private GameObject potentialGO;

		private GameObject lockedGO;

		[SerializeField]
		private GameObject canvasTargetPotential;

		private void EnableGameObjectsForLockingMode(CanvasInteractionController.LockingMode locking_mode)
		{
			foreach (CanvasTarget current in CanvasTracker.CanvasTargets)
			{
				if (current.enabledOnPostLockMode && locking_mode == CanvasInteractionController.LockingMode.PostLock)
				{
					current.lockedToTracker = true;
				}
				else if (current.enabledOnPreLockMode && locking_mode == CanvasInteractionController.LockingMode.PreLock)
				{
					current.lockedToTracker = true;
				}
				else
				{
					current.lockedToTracker = false;
				}
			}
			if (locking_mode == CanvasInteractionController.LockingMode.PostLock)
			{
				this.canvasTargetPotential.SetActive(false);
			}
			else if (locking_mode == CanvasInteractionController.LockingMode.PreLock)
			{
				this.canvasTargetPotential.SetActive(true);
			}
		}

		private void Start()
		{
			this.EnableGameObjectsForLockingMode(CanvasInteractionController.LockingMode.PreLock);
		}

		private void Update()
		{
			if (!MetaSingleton<CanvasTracker>.Instance.RectanglesDetected)
			{
				MetaSingleton<CanvasTracker>.Instance.UnlockTarget();
			}
			if (!MetaSingleton<CanvasTracker>.Instance.TargetLocked && MetaSingleton<CanvasTracker>.Instance.RectanglesDetected)
			{
				foreach (CanvasTarget current in CanvasTracker.CanvasTargets)
				{
					if (current.IsStable)
					{
						MetaSingleton<CanvasTracker>.Instance.LockTarget();
						break;
					}
				}
			}
			if (MetaSingleton<CanvasTracker>.Instance.TargetLocked)
			{
				this.EnableGameObjectsForLockingMode(CanvasInteractionController.LockingMode.PostLock);
			}
			else
			{
				this.EnableGameObjectsForLockingMode(CanvasInteractionController.LockingMode.PreLock);
			}
		}
	}
}
