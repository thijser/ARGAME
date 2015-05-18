//----------------------------------------------------------------------------
// <copyright file="CubeClient.cs" company="Delft University of Technology">
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

public class CubeClient : MonoBehaviour
{
    private Socket client = null;

    public void Start()
    {
        IPHostEntry hostInfo = Dns.GetHostEntry("127.0.0.1");
        IPAddress address = hostInfo.AddressList[0];
        IPEndPoint remoteEP = new IPEndPoint(address, 11000);

        this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.client.Connect(remoteEP);
        this.client.Blocking = false;
    }

    public void Update()
    {
        try
        {
            byte[] buffer = new byte[256];
            int len = this.client.Receive(buffer);

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
        }
        catch (SocketException exception)
        {
            Debug.LogException(exception);
        }
    }
}
