//FIXME: Replace all tabs in this file with sequences of spaces
﻿﻿﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	private const string typeName = "Lasergame";
	private const string gameName = "BEP";
	private HostData[] hostList;

	private void StartServer()
	{
		MasterServer.ipAddress = "127.0.0.1";
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
	}
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			//FIXME: Add curly braces around this if-statement
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();

				//FIXME: Add curly braces around this if-statement
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();

			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					//FIXME: Add curly braces around this if-statement
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
	// Use this for initialization
	//FIXME: Empty method
  //       Either:
  //         - Remove this method
  //         - Explain by a comment inside the method why it is empty
	void Start () {

	}

	// Update is called once per frame
	//FIXME: Empty method
  //       Either:
  //         - Remove this method
  //         - Explain by a comment inside the method why it is empty
	void Update () {

	}
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		//FIXME: Add curly braces around this if-statement
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
	}
}
