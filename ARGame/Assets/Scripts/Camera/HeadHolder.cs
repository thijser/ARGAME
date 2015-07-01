﻿//----------------------------------------------------------------------------
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

        private int trackingIndex = -1;
        /// <summary>
        /// The <see cref="RemoteMarkerHolder"/>.
        /// </summary>
        private RemoteMarkerHolder holder;

        /// <summary>
        /// The reference <c>GameObject</c> for a player head.
        /// </summary>
        private GameObject referenceHead;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.NextTracking();
            }
        }

        public void Start()
        {
            this.referenceHead = Resources.Load("Prefabs/HEAD") as GameObject;
            this.holder = gameObject.GetComponent<RemoteMarkerHolder>();
        }

        public void OnFollowPlayerInfo(ARViewUpdate playerInfo)
        {
            this.PlacePlayerHead(this.GetPlayer(playerInfo.Id), playerInfo);
        }

        public void NextTracking()
        {
            if (this.trackingIndex + 1 > this.Heads.Values.Count)
            {
                this.trackingIndex++;
            }
            else
            {
                this.trackingIndex = 0;
            }

            int i = 0;
			Debug.Log (trackingIndex);
            foreach (RemotePlayerMarker marker in Heads.Values)
            {
                if (this.trackingIndex == i)
                {
                    this.holder.PlayerToFollow = marker;
                }

                i++;
            }
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

            Vector3 scale = new Vector3(8, 8, -8);

            Vector3 position = playerInfo.Position;
            position.Scale(scale);
            Quaternion direction = Quaternion.Euler(playerInfo.Rotation);

            if (trackingIndex == -1)
            {
                this.NextTracking();
            }

            this.GetPlayer(playerInfo.Id).RemotePosition =
                new MarkerPosition(position, direction, DateTime.Now, scale, markerId);
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
