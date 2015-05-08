using UnityEngine;
using System.Collections;

public class ServerManager : MonoBehaviour {
    private const string gameName = "ARMirrors";
    private const string gameSubName = "Level1";

	void StartServer() {
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(gameName, gameSubName);
    }

    void OnServerInitialized() {
        Debug.Log("Server initialised");
    }

    void StartClient() {
        MasterServer.RequestHostList(gameName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.HostListReceived) {
            var hostList = MasterServer.PollHostList();

            Debug.Log(hostList.Length);
        }
    }

    void OnGUI() {
        if (!Network.isClient && !Network.isServer) {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();
            if (GUI.Button(new Rect(100, 250, 250, 100), "Start Client"))
                StartClient();
        }
    }
}
