//----------------------------------------------------------------------------
// <copyright file="ClientSocket.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    /// <summary>
    /// Maintains a Socket connection to the OpenCV server.
    /// </summary>
    public class ClientSocket : MonoBehaviour
    {
        /// <summary>
        /// The size of a single packet.
        /// <para>
        /// The size of a single PositionUpdate is equal to the size 
        /// of 2 floats and 2 integers. This is 3 * 4 bytes + 8 bytes = 20 bytes.
        /// </para>
        /// </summary>
        public const int PacketSize = 20;

        /// <summary>
        /// The maximum amount of updates to read in a single step.
        /// </summary>
        public const int MaxUpdates = 10;

        /// <summary>
        /// The server address.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public string ServerAddress = "localhost";

        /// <summary>
        /// The server port.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int ServerPort = 23369;

        /// <summary>
        /// The Socket used for the connection.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// The end point of the Socket connection.
        /// </summary>
        private IPEndPoint endPoint;

        /// <summary>
        /// The buffer used to receive the raw data.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// Initializes the Socket and connects to the server.
        /// </summary>
        public void Start()
        {
            this.buffer = new byte[PacketSize];

            IPAddress[] addresses = Dns.GetHostEntry(this.ServerAddress).AddressList;
            if (addresses.Length == 0)
            {
                Debug.LogError("Host is unavailable. No IP addresses found for " + this.ServerAddress);
                return;
            }

            IPAddress address = addresses[0];
            this.endPoint = new IPEndPoint(address, this.ServerPort);

            // Acquires permission to use a Socket for the desired connection.
            SocketPermission permission = new SocketPermission(System.Security.Permissions.PermissionState.Unrestricted);
            permission.Demand();

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = false;
            this.socket.ReceiveTimeout = 10000;
            this.socket.Connect(this.endPoint);
            Debug.Log("Socket connected to " + this.endPoint.Address);
        }

        /// <summary>
        /// Retrieves the PositionUpdates from the server and broadcasts the messages.
        /// </summary>
        public void Update()
        {
            this.ReadAllUpdates();
        }

        /// <summary>
        /// Disconnects the socket.
        /// </summary>
        public void DisconnectSocket()
        {
            this.socket.Disconnect(false);
        }

        /// <summary>
        /// Reads all updates and sends the "OnPositionUpdate" message.
        /// </summary>
        /// <returns>The amount of updates parsed.</returns>
        public int ReadAllUpdates()
        {
            int count = 0;
            while (this.socket.Available >= PacketSize && count < MaxUpdates)
            {
                PositionUpdate update = this.ReadUpdate();
                if (update == null)
                {
                    return count;
                }

                this.SendMessage(
                    "OnPositionUpdate",
                    update,
                    SendMessageOptions.DontRequireReceiver);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Reads a single PositionUpdate from the Socket.
        /// <para>
        /// If not enough data is available, this method may return null.
        /// </para>
        /// </summary>
        /// <returns>The PositionUpdate.</returns>
        public PositionUpdate ReadUpdate()
        {
            int received = this.socket.Receive(this.buffer, PacketSize, SocketFlags.None);
            if (received < PacketSize)
            {
                Debug.Log("Received not enough bytes: " + received);
                return null;
            }

            float x = BitConverter.ToSingle(this.buffer, 0);
            float y = BitConverter.ToSingle(this.buffer, 4);
            int id = BitConverter.ToInt32(this.buffer, 8);
            long timestamp = BitConverter.ToInt64(this.buffer, 12);

            PositionUpdate update = new PositionUpdate(x, y, id, timestamp);
            return update;
        }
    }
}
