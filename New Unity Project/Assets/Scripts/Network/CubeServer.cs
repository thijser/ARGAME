using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class CubeServer : MonoBehaviour {
    private Socket client = null;

    void Start() {
        IPHostEntry ipHostInfo = Dns.Resolve("127.0.0.1");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(localEndPoint);
        listener.Listen(1);

        client = listener.Accept();
    }

    void Update() {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        string msg = pos.x + "|" + pos.y + "|" + pos.z + "|" + rot.x + "|" + rot.y + "|" + rot.z + "|" + rot.w;
        byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

        client.Send(data);
    }
}
