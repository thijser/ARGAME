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
        /// The minimum size of a single packet.
        /// </summary>
        public const int MinPacketSize = 5;

        /// <summary>
        /// The maximum size of a single packet.
        /// <para>
        /// This is used as the size of the message buffer.
        /// </para>
        /// </summary>
        public const int MaxPacketSize = 17;

        /// <summary>
        /// The maximum amount of updates to read in a single step.
        /// </summary>
        public const int MaxUpdates = 10;

        /// <summary>
        /// The server timeout in milliseconds.
        /// </summary>
        public const long Timeout = 2000;

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
        /// Time of last server response.
        /// </summary>
        private DateTime timestamp;

        /// <summary>
        /// Initializes the Socket and connects to the server.
        /// </summary>
        public void Start()
        {
            this.buffer = new byte[MaxPacketSize];

            IPAddress[] addresses = Dns.GetHostEntry(this.ServerAddress).AddressList;
            if (addresses.Length == 0)
            {
                Debug.LogError("Host is unavailable. No IP addresses found for " + this.ServerAddress);
                return;
            }

            IPAddress address = addresses[0];
            this.endPoint = new IPEndPoint(address, this.ServerPort);

            // Acquires permission to use a Socket for the desired connection.
            SocketPermission permission = new SocketPermission(
                    System.Security.Permissions.PermissionState.Unrestricted);
            permission.Demand();

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = false;
            this.socket.ReceiveTimeout = 10000;
            this.socket.Connect(this.endPoint);
            Debug.Log("Socket connected to " + this.endPoint.Address);
            this.timestamp = DateTime.Now;
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
            while (this.socket.Available >= MinPacketSize && count < MaxUpdates)
            {
                AbstractUpdate update = this.ReadMessage();
                if (update == null)
                {
                    return count;
                }

                this.timestamp = DateTime.Now;

                if (update.Type == UpdateType.DeletePosition || update.Type == UpdateType.UpdatePosition)
                {
                    this.SendMessage(
                        "OnPositionUpdate",
                        (PositionUpdate) update,
                        SendMessageOptions.DontRequireReceiver);
                    count++;
                }
                
                else if (update.Type == UpdateType.UpdateRotation)
                {
                    this.SendMessage(
                        "OnRotationUpdate",
                        (RotationUpdate) update,
                        SendMessageOptions.DontRequireReceiver);
                    count++;
                }
            }

            long duration = (DateTime.Now - this.timestamp).Milliseconds;
            if (count == 0 && duration > Timeout)
            {
                // Assume we lost connection: Reset this ClientSocket instance.
                this.DisconnectSocket();
                this.Start();
            }

            return count;
        }

        /// <summary>
        /// Reads a single PositionUpdate message from the Socket.
        /// <para>
        /// If not enough data is available, this method may return null.
        /// </para>
        /// </summary>
        /// <returns>The PositionUpdate.</returns>
        public AbstractUpdate ReadMessage()
        {
            int received = this.socket.Receive(this.buffer, 1, SocketFlags.None);
            if (received < 1)
            {
                Debug.Log("Received not enough bytes: " + received);
                return null;
            }

            byte type = this.buffer[0];
            switch ((UpdateType)type)
            {
                case UpdateType.DeletePosition:
                    return this.ReadDelete();
                case UpdateType.UpdatePosition:
                    return this.ReadUpdatePosition();
                case UpdateType.Ping:
                    return new PositionUpdate(UpdateType.Ping, new Vector2(0, 0), 0, -1);
                case UpdateType.UpdateRotation:
                    return this.ReadUpdateRotation();
                default:
                    Debug.LogWarning("Received invalid type: " + type);
                    return null;
            }
        }

        /// <summary>
        /// Reads a <c>Update</c> type PositionUpdate message.
        /// </summary>
        /// <returns>The PositionUpdate.</returns>
        public PositionUpdate ReadUpdatePosition()
        {
            int received = this.socket.Receive(this.buffer, 16, SocketFlags.None);
            if (received < 16)
            {
                return null;
            }

			float x = this.ReadFloat(0);
			float y = this.ReadFloat(4);
            Vector2 coordinate = new Vector2(x, y);
			float rotation = this.ReadFloat(8);
			int id = this.ReadInt(12);
            return new PositionUpdate(UpdateType.UpdatePosition, coordinate, rotation, id);
        }

        /// <summary>
        /// Reads a <c>Delete</c> type PositionUpdate message.
        /// </summary>
        /// <returns>The PositionUpdate.</returns>
        public PositionUpdate ReadDelete()
        {
            int received = this.socket.Receive(this.buffer, 4, SocketFlags.None);
            if (received < 4)
            {
                return null;
            }

			int id = this.ReadInt(0);
            return new PositionUpdate(UpdateType.DeletePosition, new Vector2(0, 0), 0, id);
        }

        public RotationUpdate ReadUpdateRotation()
        {
            int received = this.socket.Receive(this.buffer, 8, SocketFlags.None);
            if (received < 8)
            {
                return null;
            }

			int id = this.ReadInt(0);
			float rotation = this.ReadFloat(4);
            return new RotationUpdate(UpdateType.UpdateRotation, rotation, id);
        }

		private float ReadFloat(int offset) 
		{
			byte[] bytes =BitConverter.GetBytes(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(this.buffer, offset)));
			return BitConverter.ToSingle(bytes, 0);
		}

		private int ReadInt(int offset)
		{
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(this.buffer, offset));
		}
    }
}
