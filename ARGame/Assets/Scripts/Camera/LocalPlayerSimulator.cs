namespace Camera
{
    using Network;
    using UnityEngine;

    /// <summary>
    /// Simulates a local player with the intent to test
    /// the behavior of the remote camera following functionality.
    /// </summary>
    public class LocalPlayerSimulator : MonoBehaviour
    {
        /// <summary>
        /// The simulated player Id.
        /// </summary>
        public int PlayerId = 1001;

        /// <summary>
        /// Sends an <see cref="ARViewUpdate"/> with the current position 
        /// and rotation.
        /// </summary>
        public void Update()
        {
            Vector3 position = this.transform.localPosition;
            position = new Vector3(position.x, position.z, -position.y);

            ARViewUpdate update = new ARViewUpdate(this.PlayerId, position, this.transform.eulerAngles);
            this.SendMessageUpwards("OnFollowPlayerInfo", update);
        }
    }
}
