namespace Camera
{
    using Core;
    using Level;
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
        /// The size of the simulated board.
        /// </summary>
        public Vector2 SimulatedBoardSize = new Vector2(10f, 12f);

        /// <summary>
        /// The simulated player Id.
        /// </summary>
        public int PlayerId = 1001;

        /// <summary>
        /// Initializes the LocalPlayerSimulator and simulates a board.
        /// </summary>
        public void Start()
        {
            HeadHolder holder = this.transform.parent.GetComponent<HeadHolder>();
            holder.GetPlayer(this.PlayerId);

            this.StartCoroutine(this.transform.parent.GetComponent<BoardResizer>().UpdateBoardSize(this.SimulatedBoardSize));
            
            LevelManager manager = this.transform.parent.GetComponent<LevelManager>();
            manager.BoardSize = this.SimulatedBoardSize;
            manager.RestartLevel();
        }

        /// <summary>
        /// Sends an <see cref="ARViewUpdate"/> with the current position 
        /// and rotation.
        /// </summary>
        public void Update()
        {
            AbstractUpdate update = new ARViewUpdate(this.PlayerId, this.transform.position, this.transform.eulerAngles);
            this.SendMessageUpwards("OnServerUpdate", update);
        }
    }
}
