namespace Core
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Disables GameObjects when they collide with Colliders.
    /// <para>
    /// This MonoBehaviour is intended to be attached to GameObjects that are bound to markers.
    /// By disabling behaviours on such objects when they collide with others, we can prevent 
    /// unexpected behaviour.
    /// </para>
	/// <para>
	/// For this class to work, The object to which this is applied must have a convex Collider,
	/// and a Rigidbody. The Rigidbody's Collision Detection must be set to Continuous, and 
	/// the Constraints should freeze all values.
	/// </para>
    /// </summary>
    public class CollisionBehaviour : MonoBehaviour
    {
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Core.CollisionBehaviour"/> 
		/// is colliding with another object.
		/// </summary>
		public bool Colliding { get; set; }

        /// <summary>
        /// Enables or disables Behaviours based on whether this GameObject collides with
        /// other objects.
        /// </summary>
        public void Update()
        {
            this.SetEnableBehaviours(!this.Colliding);
        }

		/// <summary>
		/// Sets the <c>Colliding</c> property to <c>true</c>.
		/// </summary>
		/// <param name="coll">The parameter is ignored.</param>
        public void OnCollisionEnter(Collision coll) 
        {
			this.Colliding = true;
        }

		/// <summary>
		/// Sets the <c>Colliding</c> property to <c>false</c>.
		/// </summary>
		/// <param name="coll">The parameter is ignored.</param>
		public void OnCollisionExit(Collision coll)
		{
			this.Colliding = false;
		}

        /// <summary>
        /// Sets the enabled state of all ILaserReceivers in this GameObject to <c>enable</c>.
        /// </summary>
        /// <param name="enable">True to enable all behaviours, false to disable.</param>
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