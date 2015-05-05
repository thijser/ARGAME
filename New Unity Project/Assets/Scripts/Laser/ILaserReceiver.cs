namespace Laser
{
    /// <summary>
    /// An object that is manipulated by a Laser beam.
    /// </summary>
    /// <seealso cref="Laser.Laser" />
    public interface ILaserReceiver 
    {
        /// <summary>
        /// Called every time the object is hit by a laser beam.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments.</param>
        void OnLaserHit(object sender, HitEventArgs args);
    }
}
