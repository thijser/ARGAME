//----------------------------------------------------------------------------
// <copyright file="ServerManager.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace NetworkPhoton
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Manages server connections.
    /// </summary>
    public class ServerManager : MonoBehaviour
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
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void StartServer()
        {
            Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
            MasterServer.RegisterHost(GameName, GameSubName);
        }

        /// <summary>
        /// Does nothing. Called by Unity once the server is initialized.
        /// </summary>
        public void OnServerInitialized()
        {
            Debug.Log("Server initialised");
        }

        /// <summary>
        /// Requests the list of Hosts.
        /// </summary>
        public void StartClient()
        {
            MasterServer.RequestHostList(GameName);
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

                Debug.Log(hostList.Length);
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
            if (!Network.isClient && !Network.isServer)
            {
                if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                {
                    this.StartServer();
                }

                if (GUI.Button(new Rect(100, 250, 250, 100), "Start Client"))
                {
                    this.StartClient();
                }
            }
            else if (Network.isClient)
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