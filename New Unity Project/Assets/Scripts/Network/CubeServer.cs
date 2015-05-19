//----------------------------------------------------------------------------
// <copyright file="CubeServer.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class CubeServer : MonoBehaviour
{
    private Socket client = null;

    public void Start()
    {
        IPHostEntry hostInfo = Dns.GetHostEntry("127.0.0.1");
        IPAddress address = hostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(address, 11000);

        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(localEndPoint);
        listener.Listen(1);

        this.client = listener.Accept();
    }

    public void Update()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        string msg = pos.x + "|" + pos.y + "|" + pos.z + "|" + rot.x + "|" + rot.y + "|" + rot.z + "|" + rot.w;
        byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

        this.client.Send(data);
    }
}
