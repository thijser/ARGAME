//----------------------------------------------------------------------------
// <copyright file="CollisionBehaviour.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using UnityEngine;

    /// <summary>
    /// Disables GameObjects when they collide with Colliders.
    /// <para>
    /// This MonoBehavior is intended to be attached to GameObjects that are bound to markers.
    /// By disabling behaviors on such Objects when they collide with others, we can prevent 
    /// unexpected behavior.
    /// </para>
    /// <para>
    /// For this class to work, The UnityEngine.Object to which this is applied must have a convex Collider,
    /// and a <c>Rigidbody</c>. The <c>Rigidbody</c>'s Collision Detection must be set to Continuous, and 
    /// the Constraints should freeze all values.
    /// </para>
    /// </summary>
    public class CollisionBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Core.CollisionBehaviour"/> 
        /// is colliding with another UnityEngine.Object.
        /// </summary>
        public bool Colliding { get; set; }

        /// <summary>
        /// Initializes this <see cref="Core.CollisionBehaviour"/>.
        /// </summary>
        public virtual void Start()
        {
            this.Colliding = false;
        }

        /// <summary>
        /// Enables or disables Behaviors based on whether this GameObject collides with
        /// other Objects.
        /// </summary>
        public virtual void Update()
        {
            this.SetEnableBehaviours(!this.Colliding);
        }

        /// <summary>
        /// Sets the <c>Colliding</c> property to <c>true</c>.
        /// </summary>
        /// <param name="collision">The parameter is ignored.</param>
        public virtual void OnCollisionEnter(Collision collision)
        {
            this.Colliding = true;
        }

        /// <summary>
        /// Sets the <c>Colliding</c> property to <c>false</c>.
        /// </summary>
        /// <param name="collision">The parameter is ignored.</param>
        public virtual void OnCollisionExit(Collision collision)
        {
            this.Colliding = false;
        }

        /// <summary>
        /// Sets the enabled state of all ILaserReceivers in this GameObject to <c>enable</c>.
        /// </summary>
        /// <param name="enable">True to enable all behaviors, false to disable.</param>
        public void SetEnableBehaviours(bool enable)
        {
            foreach (ILaserReceiver receiver in gameObject.GetComponents<ILaserReceiver>())
            {
                MonoBehaviour script = receiver as MonoBehaviour;

                if (script != null)
                {
                    script.enabled = enable;
                }
            }
        }
    }
}