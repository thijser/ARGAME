// Decompiled with JetBrains decompiler
// Type: Meta.CanvasInteractionController
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
  internal class CanvasInteractionController : MonoBehaviour
  {
    private GameObject potentialGO;
    private GameObject lockedGO;
    [SerializeField]
    private GameObject canvasTargetPotential;

    public CanvasInteractionController()
    {
      base.\u002Ector();
    }

    private void EnableGameObjectsForLockingMode(CanvasInteractionController.LockingMode locking_mode)
    {
      using (List<CanvasTarget>.Enumerator enumerator = CanvasTracker.CanvasTargets.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          CanvasTarget current = enumerator.Current;
          current.lockedToTracker = current.enabledOnPostLockMode && locking_mode == CanvasInteractionController.LockingMode.PostLock || current.enabledOnPreLockMode && locking_mode == CanvasInteractionController.LockingMode.PreLock;
        }
      }
      if (locking_mode == CanvasInteractionController.LockingMode.PostLock)
      {
        this.canvasTargetPotential.SetActive(false);
      }
      else
      {
        if (locking_mode != CanvasInteractionController.LockingMode.PreLock)
          return;
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
        MetaSingleton<CanvasTracker>.Instance.UnlockTarget();
      if (!MetaSingleton<CanvasTracker>.Instance.TargetLocked && MetaSingleton<CanvasTracker>.Instance.RectanglesDetected)
      {
        using (List<CanvasTarget>.Enumerator enumerator = CanvasTracker.CanvasTargets.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            if (enumerator.Current.IsStable)
            {
              MetaSingleton<CanvasTracker>.Instance.LockTarget();
              break;
            }
          }
        }
      }
      if (MetaSingleton<CanvasTracker>.Instance.TargetLocked)
        this.EnableGameObjectsForLockingMode(CanvasInteractionController.LockingMode.PostLock);
      else
        this.EnableGameObjectsForLockingMode(CanvasInteractionController.LockingMode.PreLock);
    }

    private enum LockingMode
    {
      PreLock,
      PostLock,
    }
  }
}
