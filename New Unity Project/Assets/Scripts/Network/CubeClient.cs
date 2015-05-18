using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class CubeClient : MonoBehaviour {
    Socket client = null;

    void Start() {
        IPHostEntry ipHostInfo = Dns.Resolve("127.0.0.1");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(remoteEP);
        client.Blocking = false;
    }

    void Update() {
        try {
            byte[] buffer = new byte[256];
            int len = client.Receive(buffer);

            string[] parts = System.Text.Encoding.ASCII.GetString(buffer, 0, len).Split('|');

            float px = float.Parse(parts[0]);
            float py = float.Parse(parts[1]);
            float pz = float.Parse(parts[2]);

            float rx = float.Parse(parts[3]);
            float ry = float.Parse(parts[4]);
            float rz = float.Parse(parts[5]);
            float rw = float.Parse(parts[6]);

            transform.position = new Vector3(px, py, pz);
            transform.rotation = new Quaternion(rx, ry, rz, rw);
        } catch (SocketException) {
            // Do nothing
        }
    }
}
