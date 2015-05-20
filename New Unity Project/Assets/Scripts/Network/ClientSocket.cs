namespace Network
{
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    public class ClientSocket : MonoBehaviour 
    {
        
        /// <summary>
        /// The server address.
        /// </summary>
        public string ServerAddress = "localhost";

        /// <summary>
        /// The server port.
        /// </summary>
        public int ServerPort = 23369;

        /// <summary>
        /// The Socket used for the connection.
        /// </summary>
        private Socket socket; 

        /// <summary>
        /// The SocketPermission required for making the 
        /// connection.
        /// </summary>
        private SocketPermission permission;

        /// <summary>
        /// The end point of the Socket connection.
        /// </summary>
        private IPEndPoint endPoint;

        /// <summary>
        /// Initializes the Socket and connects to the server.
        /// </summary>
        public void Start() 
        {
            this.permission = new SocketPermission(
                NetworkAccess.Connect, 
                TransportType.Tcp, 
                "",
                SocketPermission.AllPorts);
            
            IPAddress[] addresses = Dns.GetHostEntry(this.ServerAddress).AddressList;
            if (addresses.Length == 0) 
            {
                Debug.LogError("Host is unavailable. No IP addresses found for " + this.ServerAddress);
                return;
            }

            IPAddress address = addresses[0];
            this.endPoint = new IPEndPoint(address, this.ServerPort);
            this.socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(this.endPoint);
        }
		
        public void Update() 
        {
            
        }


    }
}