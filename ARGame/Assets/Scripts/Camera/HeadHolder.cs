//----------------------------------------------------------------------------
// <copyright file="HeadHolder.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Camera
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Network;
    using Projection;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// Keeps track of local players and updates their positions in the world.
    /// </summary>
    public class HeadHolder : MonoBehaviour
    {
        /// <summary>
        /// The offset used for player Ids.
        /// </summary>
        public const int PlayerIdOffset = 9000;

        /// <summary>
        /// The array with the colors to use for players.
        /// <para>
        /// The colors are assigned to players in order: the first player gets the 
        /// first color in this array, the second player the second color, and so on.
        /// </para>
        /// <para>
        /// If there are more players than defined colors, the array is traversed again
        /// to prevent exceptions. This does mean that there will be multiple players with 
        /// the same color.
        /// </para>
        /// </summary>
        private static readonly Color[] PlayerColors = new Color[]
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow
        };
        
        /// <summary>
        /// A Dictionary mapping player id to a player marker.
        /// </summary>
        private Dictionary<int, RemotePlayerMarker> players = new Dictionary<int, RemotePlayerMarker>();

        /// <summary>
        /// The index of the currently tracked player.
        /// </summary>
        private int trackingIndex = -1;

        /// <summary>
        /// The <see cref="RemoteMarkerHolder"/>.
        /// </summary>
        private RemoteMarkerHolder holder;

        /// <summary>
        /// The reference <see cref="GameObject"/> for a player head.
        /// </summary>
        private GameObject referenceHead;

        /// <summary>
        /// Gets or sets the <see cref="RemotePlayerMarker"/> that simulates the overview camera.
        /// </summary>
        public RemotePlayerMarker OverviewMarker { get; set; }

        /// <summary>
        /// Initializes this <see cref="HeadHolder"/> instance.
        /// </summary>
        public void Start()
        {
            this.referenceHead = Resources.Load("Prefabs/HEAD") as GameObject;
            this.holder = gameObject.GetComponent<RemoteMarkerHolder>();
        }

        /// <summary>
        /// Changes the view of the local player if an appropriate key is pressed.
        /// <para>
        /// Pressing the space bar changes the view to the next player's local view,
        /// while pressing `O` changes back to the overview camera.
        /// </para>
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.NextTracking();
            }
            else if (Input.GetKeyDown(KeyCode.O) && this.OverviewMarker != null)
            {
                // `O` for overview camera.
                this.holder.PlayerToFollow = this.OverviewMarker;
            }
        }

        /// <summary>
        /// Updates the position of a player as indicated by the argument 
        /// <see cref="ARViewUpdate"/>.
        /// </summary>
        /// <param name="update">The <see cref="ARViewUpdate"/>.</param>
        public void OnFollowPlayerInfo(ARViewUpdate update)
        {
            this.PlacePlayerHead(this.GetPlayer(update.Id), update);
        }

        /// <summary>
        /// Changes the tracked player to the next player in the sequence.
        /// </summary>
        public void NextTracking()
        {
            RemotePlayerMarker marker = null;
            bool takeNextEntry = false;
            foreach (var entry in this.players)
            {
                if (takeNextEntry)
                {
                    marker = entry.Value;
                }

                if (entry.Value.Id == this.trackingIndex)
                {
                    takeNextEntry = true;
                }
            }

            if (marker == null && this.players.Count > 0)
            {
                // We reached the end, so take the first one.
                marker = this.players.First().Value;
            }

            if (marker != null)
            {
                this.Follow(marker);
                trackingIndex = marker.Id;
            }
        }

        /// <summary>
        /// Gets a local player by its id.
        /// </summary>
        /// <param name="id">The player id.</param>
        /// <returns>The <see cref="RemotePlayerMarker"/> that represents the player.</returns>
        public RemotePlayerMarker GetPlayer(int id)
        {
            if (!this.players.ContainsKey(id))
            {
                if (this.referenceHead == null)
                {
                    this.referenceHead = Resources.Load("Prefabs/HEAD") as GameObject;
                }

                GameObject head = GameObject.Instantiate(this.referenceHead);
                RemotePlayerMarker marker = head.GetComponent<RemotePlayerMarker>();
                Assert.IsNotNull(marker, "Reference Player Marker has no `RemotePlayerMarker` script attached");
                
                this.players.Add(id, marker);
                marker.PlayerColor = PlayerColors[this.players.Count % PlayerColors.Length];
                marker.transform.SetParent(this.transform);

                marker.Id = PlayerIdOffset + id;
                if (this.holder == null)
                {
                    this.holder = gameObject.GetComponent<RemoteMarkerHolder>();
                    Assert.IsNotNull(this.holder, "No RemoteMarkerHolder attached to HeadHolder's GameObject.");
                }

                this.holder.AddMarker(marker);
            }

            return this.players[id];
        }

        /// <summary>
        /// Places the given <see cref="RemotePlayerMarker"/> at the position and 
        /// rotation indicated by the <see cref="ARViewUpdate"/>.
        /// </summary>
        /// <param name="head">The <see cref="RemotePlayerMarker"/>.</param>
        /// <param name="playerInfo">The <see cref="ARViewUpdate"/>.</param>
        public void PlacePlayerHead(RemotePlayerMarker head, ARViewUpdate playerInfo)
        {
            Assert.IsNotNull(head);
            Assert.IsNotNull(playerInfo);
            int markerId = playerInfo.Id + PlayerIdOffset;

            Vector3 position = playerInfo.Position;
            Quaternion direction = Quaternion.LookRotation(playerInfo.Rotation - position);

            if (this.trackingIndex == -1)
            {
                this.NextTracking();
            }

            this.GetPlayer(playerInfo.Id).RemotePosition =
                new MarkerPosition(position, direction, DateTime.Now, Vector3.one, markerId);
        }

        /// <summary>
        /// Starts following the given RemotePlayerMarker as camera origin.
        /// </summary>
        /// <param name="player">The player to follow, not null.</param>
        public void Follow(RemotePlayerMarker player)
        {
            Assert.IsNotNull(player, "Attempt to follow null");
            if (this.OverviewMarker == null) {
                this.OverviewMarker = this.holder.PlayerToFollow;
            }

            this.holder.PlayerToFollow = player;
        }
    }
}
