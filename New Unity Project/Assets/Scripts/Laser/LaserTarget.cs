namespace Laser
{
  using System;
  using UnityEngine;

  /// <summary>
  /// A Laser target that loads the next level when hit with a Laser beam.
  /// </summary>
  public class LaserTarget : MonoBehaviour, ILaserReceiver
  {
    /// <summary>
    /// Gets a value indicating whether the target is fully opened.
    /// </summary>
    public bool FullyOpened { get; private set; }

    /// <summary>
    /// Consumes the Laser beam and opens the target one step.
    /// </summary>
    /// <param name="sender">The object that sent this event</param>
    /// <param name="args">The arguments that describe the event</param>
    public void OnLaserHit(object sender, HitEventArgs args)
    {
      this.AnimateStep();
      if (this.FullyOpened)
      {
        this.LoadNextLevel();
      }
    }

    /// <summary>
    /// Animates the laser target one step.
    /// </summary>
    public void AnimateStep()
    {
      // TODO: Animate the laser target
      throw new NotSupportedException("Laser target animation is not yet implemented");
    }

    /// <summary>
    /// Loads the next level.
    /// </summary>
    public void LoadNextLevel()
    {
      // TODO: Load the next Level
      throw new NotSupportedException("Loading next level is not yet implemented");
    }
  }
}
