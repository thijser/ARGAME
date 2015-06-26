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
        public const int MaxPacketSize = 30;

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

            this.socket = new Socket(this.endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = false;
            this.socket.ReceiveTimeout = 10000;
            this.socket.Connect(this.endPoint);
            Debug.Log("Socket connected to " + this.endPoint.Address);
            this.timestamp = DateTime.Now;

            this.SendMessage("OnSocketStart", this.endPoint, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Retrieves the PositionUpdates from the server and broadcasts the messages.
        /// </summary>
        public void Update()
        {
            if (this.socket != null)
            {
                this.ReadAllUpdates();
            }
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

            if (this.socket == null)
            {
                // This may sometimes happen without any explanation. Our only option is to restart
                // the socket connection.
                Debug.Log("Restarted broken socket connection");
                this.Start();
                return 0;
            }

            while (this.socket.Available >= MinPacketSize && count < MaxUpdates)
            {
                AbstractUpdate update = this.ReadMessage();
                if (update == null)
                {
                    return count;
                }

                this.timestamp = DateTime.Now;
                if (update.Type != UpdateType.Ping)
                {
                    this.SendMessage("OnServerUpdate", update, SendMessageOptions.DontRequireReceiver);
                }

                count++;
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
                    received = this.socket.Receive(this.buffer, 4, SocketFlags.None);
                    return MessageProcessor.ReadDelete(this.buffer, received);
                case UpdateType.UpdatePosition:
                    received = this.socket.Receive(this.buffer, 16, SocketFlags.None);                   
                    return MessageProcessor.ReadUpdatePosition(this.buffer, received);
                case UpdateType.Ping:
                    return new PingUpdate();
                case UpdateType.UpdateRotation:
                    received = this.socket.Receive(this.buffer, 8, SocketFlags.None);
                    return MessageProcessor.ReadUpdateRotation(this.buffer, received);
                case UpdateType.UpdateLevel:
                    received = this.socket.Receive(this.buffer, 12, SocketFlags.None);
                    return MessageProcessor.ReadUpdateLevel(this.buffer, received);
                case UpdateType.UpdateARView:
                    received = this.socket.Receive(this.buffer, 28, SocketFlags.None);
				Debug.Log ("I see");
                    return MessageProcessor.ReadARViewUpdate(this.buffer, received);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Called whenever the remote player rotates an object.
        /// </summary>
        /// <param name="update">The RotationUpdate describing the change.</param>
        public void OnRotationChanged(RotationUpdate update)
        {
            this.socket.Send(MessageProcessor.WriteRotationUpdate(update));
        }

        /// <summary>
        /// Called whenever one local player manages to complete the level.
        /// </summary>
        /// <param name="update">The LevelUpdate describing the change.</param>
        public void OnLevelCompleted(LevelUpdate update)
        {
            this.socket.Send(MessageProcessor.WriteLevelUpdate(update));
        }

        /// <summary>
        /// Called whenever a local player changes its position.
        /// </summary>
        /// <param name="update"></param>
        public void OnSendPosition(ARViewUpdate update)
        {
            this.socket.Send(MessageProcessor.WriteARViewUpdate(update));
            Debug.LogError("Sending message: " + update);
        }
    }
}
