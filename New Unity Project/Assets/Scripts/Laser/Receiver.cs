//----------------------------------------------------------------------------
// <copyright file="Receiver.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser 
{
    using UnityEngine;
    
    /// <summary>
    /// <para>
    /// Dynamic Receiver ready to be used in the Unity Editor.
    /// Because the Unity editor can only deal with specific types
    /// and not with interfaces, this class enables registration of
    /// ILaserReceiver instances from within the Unity Editor.
    /// </para>
    /// <para>
    /// To use this class, add it to a gameObject in the Editor.
    /// Also add the class you want to use as a script.
    /// Then drag the script of the class you want to use into
    /// the "Laser Behavior" field in the Unity Editor.
    /// This will cause the desired behavior to be invoked when
    /// the object is hit with a Laser beam.
    /// </para>
    /// </summary>
    public class Receiver : MonoBehaviour, ILaserReceiver 
    {
        /// <summary>
        /// The ILaserReceiver to call when this object is hit by a Laser beam.
        /// </summary>
        public MonoBehaviour LaserBehavior;

        /// <summary>
        /// Calls the delegate LaserBehavior object if it is a valid
        /// ILaserReceiver instance. Logs an error message otherwise.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="args">The HitEventArgs describing the event</param>
        public void OnLaserHit(object sender, HitEventArgs args) 
        {
            ILaserReceiver receiver = this.LaserBehavior as ILaserReceiver;
            if (receiver != null) 
            {
                receiver.OnLaserHit(this, args);
            } 
            else 
            {
                Debug.LogError("LaserBehaviour set to a non-ILaserReceiver script");
            }
        }
    }
}
