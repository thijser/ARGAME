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
        /// A Dictionary mapping player id to Head.
        /// </summary>
        private Dictionary<int, RemotePlayerMarker> Heads = new Dictionary<int, RemotePlayerMarker>();
        
        /// <summary>
        /// The <see cref="RemoteMarkerHolder"/>.
        /// </summary>
        private RemoteMarkerHolder holder;

        /// <summary>
        /// The reference <c>GameObject</c> for a player head.
        /// </summary>
        private GameObject referenceHead;

        public void Start()
        {
            this.referenceHead = Resources.Load("Prefabs/HEAD") as GameObject;
            this.holder = gameObject.GetComponent<RemoteMarkerHolder>();
        }

        public void OnFollowPlayerInfo(ARViewUpdate playerInfo)
        {
            this.PlacePlayerHead(this.GetPlayer(playerInfo.Id), playerInfo);
        }

        public RemotePlayerMarker GetPlayer(int id)
        {
            if (!this.Heads.ContainsKey(id))
            {
                GameObject head = GameObject.Instantiate(this.referenceHead);
                RemotePlayerMarker marker = head.GetComponent<RemotePlayerMarker>();
                Assert.IsNotNull(marker, "Reference Player Marker has no `RemotePlayerMarker` script attached");

                Heads.Add(id, marker);
                marker.transform.SetParent(this.transform);

                marker.Id = PlayerIdOffset + id;
                this.holder.AddMarker(marker);
                this.Follow(marker);
            }

            return this.Heads[id];
        }

        public void PlacePlayerHead(RemotePlayerMarker head, ARViewUpdate playerInfo)
        {
            Assert.IsNotNull(head);
            Assert.IsNotNull(playerInfo);
            int markerId = playerInfo.Id + PlayerIdOffset;

            Vector3 scale = new Vector3(8, -8, -8);

            Vector3 position = playerInfo.Position;
			position=new Vector3(position.x,position.z,position.y);
			position.Scale(scale);

            Quaternion direction = Quaternion.Euler(playerInfo.Rotation - new Vector3(0, 90, 0));
            position = direction * position;
            
            head.RemotePosition = new MarkerPosition(position, direction, DateTime.Now, 8 * Vector3.one, markerId);
        }

        /// <summary>
        /// Starts following the given RemotePlayerMarker as camera origin.
        /// </summary>
        /// <param name="player">The player to follow.</param>
        public void Follow(RemotePlayerMarker player)
        {
            this.holder.PlayerToFollow = player;
            Debug.Log("Started following player: " + (player == null ? "none" : player.Id.ToString()));
        }
    }
}
