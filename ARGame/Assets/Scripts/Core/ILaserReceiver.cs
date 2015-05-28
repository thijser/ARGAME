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
    /// <para>
    /// Implementations of this class must extend from
    /// <c>MonoBehaviour</c> to work properly. Implementations
    /// that do not may cause Exceptions at run time.
    /// </para>
    /// </summary>
    /// <seealso cref="LaserBeam.LaserBeam" />
    public interface ILaserReceiver
    {
        /// <summary>
        /// Called every time the object is Hit by a laser beam.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments.</param>
        void OnLaserHit(object sender, HitEventArgs args);
    }
}
