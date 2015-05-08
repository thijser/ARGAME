using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientManager : MonoBehaviour {
    public Transform mirror;

    private Socket client;

    void Start() {
        IPHostEntry ipHostInfo = Dns.Resolve("145.94.147.3");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 25000);

        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(remoteEP);
        client.Blocking = false;
    }

    void Update() {
        try {
            byte[] buffer = new byte[256];
            int bytesRead = client.Receive(buffer);

            if (buffer.Length > 0) {
                string[] data = Encoding.ASCII.GetString(buffer, 0, bytesRead).Split(',');

                mirror.transform.position = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                mirror.transform.rotation = new Quaternion(float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]));
            }
        } catch (SocketException e) {
            // No data, nothing to do
        }
    }
}
