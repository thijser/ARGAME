//----------------------------------------------------------------------------
// <copyright file="ServerManager.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System.Collections;
using UnityEngine;
using UNetwork = UnityEngine.Network;

/// <summary>
/// Manages server connections.
/// </summary>
public class ServerManager : MonoBehaviour
{
    // FIXME: This class shares (almost) all code with the Network.NetworkManager class.
    
    /// <summary>
    /// The name of this game.
    /// </summary>
    private const string GameName = "ARMirrors";

    /// <summary>
    /// The name of this game instance.
    /// </summary>
    private const string GameSubName = "Level1";

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void StartServer()
    {
        UNetwork.InitializeServer(4, 25000, !UNetwork.HavePublicAddress());
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
        if (!UNetwork.isClient && !UNetwork.isServer)
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
    }
}
