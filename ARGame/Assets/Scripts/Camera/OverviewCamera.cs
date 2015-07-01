namespace Camera
{
    using UnityEngine;
    using System.Collections;
    using Projection;
    using Level;
    using Network;

    /// <summary>
    /// Overview camera implemented as fake local player.
    /// </summary>
    public class OverviewCamera : MonoBehaviour
    {
        /// <summary>
        /// The simulated player Id.
        /// </summary>
        public int FakeOverviewPlayerId = 1001;

        /// <summary>
        /// The remote player marker being simulated.
        /// </summary>
        private RemotePlayerMarker marker;

        /// <summary>
        /// Initializes the LocalPlayerSimulator and simulates a board.
        /// </summary>
        public void Start()
        {
            HeadHolder holder = this.transform.parent.GetComponent<HeadHolder>();
            this.marker = holder.GetPlayer(this.FakeOverviewPlayerId);
            holder.Follow(this.marker);
        }

        /// <summary>
        /// Sends an <see cref="ARViewUpdate"/> with the current position 
        /// and rotation.
        /// </summary>
        public void Update()
        {
            LevelManager manager = this.transform.parent.GetComponent<LevelManager>();
            transform.position = new Vector3(manager.BoardSize.x / 2, 30, manager.BoardSize.y / 2);

            Vector3 position = this.transform.position;
            ARViewUpdate update = new ARViewUpdate(this.FakeOverviewPlayerId, position, this.transform.eulerAngles);
            this.SendMessageUpwards("OnFollowPlayerInfo", update);
        }
    }
}