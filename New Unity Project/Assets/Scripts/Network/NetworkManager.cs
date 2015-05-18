//----------------------------------------------------------------------------
// <copyright file="NetworkManager.cs" company="Delft University of Technology">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// Manages network communications and synchronizes
    /// game state between connected players.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        /// <summary>
        /// <para>
        /// The address of the master server.
        /// </para>
        /// <para>
        /// An empty string indicates that Unity's default master server is used.
        /// </para>
        /// </summary>
        public const string ServerAddress = "";

        /// <summary>
        /// The network port of the master server to connect to.
        /// </summary>
        public const int ServerPort = 25000;

        /// <summary>
        /// The name of this game type as provided to the Master server.
        /// </summary>
        public const string TypeName = "MirrorsAR";

        /// <summary>
        /// The maximum amount of players that can connect to the server.
        /// </summary>
        public const int MaxPlayers = 4;

        /// <summary>
        /// The available hosts.
        /// </summary>
        private HostData[] hosts;

        /// <summary>
        /// The mirrors in the scene.
        /// </summary>
        private IList<GameObject> mirrors = new List<GameObject>();

        /// <summary>
        /// The index of the current selected mirror.
        /// </summary>
        private int mirrorIndex = 0;

        /// <summary>
        /// <para>
        /// Gets a value indicating the available hosts.
        /// This will be retrieved from the master server.
        /// </para>
        /// <para>
        /// If the available hosts are not (yet) known, the
        /// value of this field is an empty array.
        /// </para>
        /// </summary>
        public ReadOnlyCollection<HostData> Hosts
        {
            get
            {
                return Array.AsReadOnly<HostData>(this.hosts);
            }
        }

        /// <summary>
        /// <para>
        /// Initializes the network as a server.
        /// </para>
        /// <para>
        /// This method may only be called once. Furthermore, once
        /// this method is called, it is not possible to call
        /// the <c>InitializeClient(HostData)</c> method while the
        /// server is running.
        /// </para>
        /// </summary>
        /// <param name="gameName">the name of the game instance</param>
        public static void InitializeServer(string gameName)
        {
            if (!Network.isServer && !Network.isClient)
            {
                Network.InitializeServer(MaxPlayers, ServerPort, !Network.HavePublicAddress());
                MasterServer.RegisterHost(TypeName, gameName);
            }
            else
            {
                Debug.LogError("Tried to initialize server with Network already initialized");
            }
        }

        /// <summary>
        /// <para>
        /// Initializes the network as a client.
        /// </para>
        /// <para>
        /// This method may only be called once. Furthermore, once
        /// this method is called, it is not possible to call
        /// the <c>InitializeServer(string)</c> method while the
        /// client is connected.
        /// </para>
        /// </summary>
        /// <param name="host">The host to connect to, not null</param>
        public static void InitializeClient(HostData host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (!Network.isServer && !Network.isClient)
            {
                Network.Connect(host);
            }
            else
            {
                Debug.LogError("Tried to initialize client with Network already initialized");
            }
        }

        /// <summary>
        /// Starts the NetworkManager.
        /// </summary>
        public void Start()
        {
            if (ServerAddress.Length > 0)
            {
                MasterServer.ipAddress = ServerAddress;
            }

            this.hosts = new HostData[0];
            foreach (GameObject obj in UnityObject.FindObjectsOfType<GameObject>())
            {
                if (obj.GetComponent<MirrorRotation>() != null)
                {
                    mirrors.Add(obj);
                }
            }
        }

        /// <summary>
        /// Produces a lobby GUI.
        /// </summary>
        public void OnGUI()
        {
            if (!Network.isClient && !Network.isServer)
            {
                if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                {
                    InitializeServer("BEP");
                }

                if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                {
                    RefreshHostList();
                }

                if (hosts != null)
                {
                    for (int i = 0; i < hosts.Length; i++)
                    {
                        if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hosts[i].gameName))
                        {
                            InitializeClient(hosts[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the host list.
        /// </summary>
        private void RefreshHostList()
        {
            MasterServer.RequestHostList(TypeName);
        }

        /// <summary>
        /// <para>
        /// Processes a master server event.
        /// </para>
        /// <para>
        /// This method updates the hosts list when the server event
        /// is equal to <c>MasterServer.HostListReceived</c>.
        /// </para>
        /// </summary>
        /// <param name="serverEvent">The event that occurred</param>
        public void OnMasterServerEvent(MasterServerEvent serverEvent)
        {
            if (serverEvent == MasterServerEvent.HostListReceived)
            {
                this.hosts = MasterServer.PollHostList();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.M) && mirrors.Count > 0 && Network.isServer)
            {
                mirrors[mirrorIndex].GetComponent<MirrorRotation>().selected = false;
                mirrorIndex++;
                mirrorIndex = mirrorIndex % mirrors.Count;
                mirrors[mirrorIndex].GetComponent<MirrorRotation>().selected = true;
            }
        }
    }
}
