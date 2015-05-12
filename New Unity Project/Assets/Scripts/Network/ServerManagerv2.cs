//----------------------------------------------------------------------------
// <copyright file="ServerManager.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
	using System;
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Manages server connections.
    /// </summary>
    public class ServerManagerv2 : MonoBehaviour
    {
        /// <summary>
        /// The name of this game.
        /// </summary>
        private const string GameName = "ARMirrors";

        /// <summary>
        /// The name of this game instance.
        /// </summary>
        private const string GameSubName = "Level1";

        /// <summary>
        /// Sets up state shared between client and server.
        /// </summary>
        public void Start()
        {
            Network.sendRate = 60;
			MasterServer.RequestHostList(GameName);
        }

        /// <summary>
        /// Does nothing. Called by Unity once the server is initialized.
        /// </summary>
        public void OnServerInitialized()
        {

        }

        /// <summary>
        /// <para>
        /// Processes the given <see cref="MasterServerEvent"/>.
        /// </para>
        /// <para>
        /// Retrieves the host list if the parameter is 
        /// <c>MasterServerEvent.HostListReceived</c>.
        /// </para>
        /// </summary>
        /// <param name="serverEvent">The MasterServerEvent that occurred.</param>
        public void OnMasterServerEvent(MasterServerEvent serverEvent)
        {
            if (serverEvent == MasterServerEvent.HostListReceived)
            {
                var hostList = MasterServer.PollHostList();

                if (hostList.Length > 0)
                {
                    Network.Connect(hostList[0]);
                }
				else 
				{
					Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
					MasterServer.RegisterHost(GameName, GameSubName);
				}
            }
        }

        /// <summary>
        /// <para>
        /// Updates the GUI.
        /// </para>
        /// <para>
        /// Shows two buttons for starting as server or client as long as the
        /// network is not initialized.
        /// </para>
        /// </summary>
        public void OnGUI()
        {
            if (Network.isClient)
            {
                GUI.Label(new Rect(20, 15, 100, 50), "Client");
            }
            else if (Network.isServer)
            {
                GUI.Label(new Rect(20, 15, 100, 50), "Server");
            }
        }
    }
}