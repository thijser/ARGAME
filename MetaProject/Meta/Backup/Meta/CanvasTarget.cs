// Decompiled with JetBrains decompiler
// Type: Meta.CanvasTarget
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class CanvasTarget : MonoBehaviour
  {
    internal bool m_kalmanFiltering;
    internal float m_KalmanVelocity;
    internal bool m_debugMode;
    internal bool m_InheritScale;
    [SerializeField]
    internal bool m_enabledOnPreLockMode;
    [SerializeField]
    internal bool m_enabledOnPostLockMode;
    private KalmanFilterClient _kalmanFilter;
    private Vector3 _originalScale;
    private Vector3 _lastPosition;
    private float _positionDelta;
    private Vector3 _smoothedPosition;
    private Quaternion _smoothedRotation;
    [SerializeField]
    private bool m_IsStable;
    internal float stabilityVelocityThreshold;
    internal float stabilityTimeThreshold;
    internal float stabilityVelocity;
    internal float stabilityTimeElapsed;
    internal float lastUnstableTime;
    private bool _lockedToTracker;

    internal bool enabledOnPostLockMode
    {
      get
      {
        return this.m_enabledOnPostLockMode;
      }
      set
      {
        this.m_enabledOnPostLockMode = value;
      }
    }

    internal bool enabledOnPreLockMode
    {
      get
      {
        return this.m_enabledOnPreLockMode;
      }
      set
      {
        this.m_enabledOnPreLockMode = value;
      }
    }

    internal bool IsStable
    {
      get
      {
        return this.m_IsStable;
      }
    }

    internal bool lockedToTracker
    {
      get
      {
        return this._lockedToTracker;
      }
      set
      {
        this._lockedToTracker = value;
      }
    }

    public CanvasTarget()
    {
      base.\u002Ector();
    }

    private void CheckStability()
    {
      this.stabilityVelocity = Time.get_deltaTime() * this.stabilityVelocityThreshold;
      if ((double) this._positionDelta > (double) this.stabilityVelocity || !MetaSingleton<CanvasTracker>.Instance.RectanglesDetected)
      {
        this.m_IsStable = false;
        this.lastUnstableTime = Time.get_time();
      }
      else
      {
        this.stabilityTimeElapsed = Time.get_time() - this.lastUnstableTime;
        if ((double) this.stabilityTimeElapsed <= (double) this.stabilityTimeThreshold)
          return;
        this.m_IsStable = true;
      }
    }

    private void SetCanvasTransform()
    {
      Quaternion rotation = CanvasTracker.GetRotation();
      Vector3 position = CanvasTracker.GetPosition();
      Vector3 scale = CanvasTracker.GetScale();
      if (!MetaSingleton<CanvasTracker>.Instance.IsValid)
        return;
      this._positionDelta = Vector3.Distance(this._lastPosition, position);
      this.CheckStability();
      if (MetaSingleton<CanvasTracker>.Instance.RectanglesDetected)
      {
        ((Component) this).get_transform().set_rotation(rotation);
        ((Component) this).get_transform().set_position(position);
      }
      if (this.m_kalmanFiltering)
      {
        this._kalmanFilter.kalmanVelocity = this.m_KalmanVelocity;
        this._kalmanFilter.KalmanFilterSmoothTransform(((Component) this).get_transform(), out this._smoothedPosition, out this._smoothedRotation);
        ((Component) this).get_transform().set_position(this._smoothedPosition);
        ((Component) this).get_transform().set_rotation(this._smoothedRotation);
      }
      this._lastPosition = ((Component) this).get_transform().get_position();
      if (this.m_InheritScale)
        ((Component) this).get_transform().set_localScale(Vector3.Scale(scale, this._originalScale));
      else
        ((Component) this).get_transform().set_localScale(this._originalScale);
    }

    private void Start()
    {
      CanvasTracker.AddTarget(this);
      this._originalScale = ((Component) this).get_transform().get_localScale();
    }

    private void LateUpdate()
    {
      if (!this.lockedToTracker)
        return;
      this.SetCanvasTransform();
    }
  }
}
