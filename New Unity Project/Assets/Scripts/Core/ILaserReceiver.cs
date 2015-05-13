//----------------------------------------------------------------------------
// <copyright file="ILaserReceiver.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    /// <summary>
    /// An object that is manipulated by a Laser beam.
    /// </summary>
    /// <seealso cref="LaserBeam.LaserBeam" />
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
