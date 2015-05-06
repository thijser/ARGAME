namespace Laser 
{
    using System;
    using UnityEngine;

    /// <summary>
    /// A Mirror that reflects Laser beams.
    /// </summary>
    public class Mirror : MonoBehaviour, ILaserReceiver 
    {
        /// <summary>
        /// Reflects the argument Laser beam and creates a new Laser beam
        /// in the reflected direction.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The EventArgs object that describes the event.</param>
        public void OnLaserHit(object sender, HitEventArgs args) 
        {
            if (args == null) 
            {
                throw new ArgumentNullException("args");
            }
            
            Vector3 reflection = Vector3.Reflect(args.Laser.Direction, args.Normal);
            args.Laser.Extend(args.Laser.Endpoint, reflection);
        }
    }
}
