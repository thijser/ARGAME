namespace Camera
{
    using Network;
    using Projection;
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
        /// The remote player marker being simulated.
        /// </summary>
        private RemotePlayerMarker marker;

        /// <summary>
        /// Initializes a RemotePlayerMarker to use.
        /// </summary>
        public void Start()
        {
            HeadHolder holder = this.transform.parent.GetComponent<HeadHolder>();
            this.marker = holder.GetPlayer(this.PlayerId);
            holder.Follow(this.marker);
        }

        /// <summary>
        /// Sends an <see cref="ARViewUpdate"/> with the current position 
        /// and rotation.
        /// </summary>
        public void Update()
        {
            Vector3 position = this.transform.localPosition;
            ARViewUpdate update = new ARViewUpdate(this.PlayerId, position, this.transform.eulerAngles);
            this.SendMessageUpwards("OnFollowPlayerInfo", update);
        }
    }
}
