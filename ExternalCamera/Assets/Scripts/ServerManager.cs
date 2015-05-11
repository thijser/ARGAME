using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerManager : MonoBehaviour {
    public Transform mirror;

    private Socket client;

    void Start() {
        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 25000);

        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(localEndPoint);
        listener.Listen(1);

        Debug.Log("waiting for client...");

        client = listener.Accept();

        Debug.Log("accepted client");
    }

    void Update() {
        string data = mirror.position.x + "," + mirror.position.y + "," + mirror.position.z + "," + mirror.rotation.x + "," + mirror.rotation.y + "," + mirror.rotation.z + "," + mirror.rotation.w;
        byte[] dataBytes = Encoding.ASCII.GetBytes(data);

        client.Send(dataBytes);

        Debug.Log(data);
    }
}