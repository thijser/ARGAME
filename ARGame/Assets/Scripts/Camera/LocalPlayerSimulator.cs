//----------------------------------------------------------------------------
// <copyright file="LocalPlayerSimulator.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
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
        public Vector2 SimulatedBoardSize = new Vector2(8f, 10f);

        /// <summary>
        /// The simulated player Id.
        /// </summary>
        public int PlayerId = 1001;

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
            this.marker = holder.GetPlayer(this.PlayerId);
            holder.Follow(this.marker);

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
            Vector3 position = this.transform.position;
            ARViewUpdate update = new ARViewUpdate(this.PlayerId, position, this.transform.eulerAngles);
            this.SendMessageUpwards("OnFollowPlayerInfo", update);
        }
    }
}
