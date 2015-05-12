//----------------------------------------------------------------------------
// <copyright file="CubeMover.cs" company="Delft University of Technology">
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
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Moves a cube through the world if the network is initialized as a server.
    /// </summary>
    public class CubeMover : MonoBehaviour
    {
        /// <summary>
        /// Updates the location of the cube.
        /// </summary>
        public void Update()
        {
            if (Network.isServer)
            {
                float t = Time.deltaTime * 90;
                transform.Rotate(0, t, 0);
            }
        }

        /// <summary>
        /// Transmits the location of the cube over the given BitStream.
        /// </summary>
        /// <param name="stream">The BitStream to serialize to.</param>
        /// <param name="info">The NetworkMessageInfo describing the connection.</param>
        public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (stream.isWriting)
            {
                Vector3 pos = transform.position;
                stream.Serialize(ref pos);
            }
            else
            {
                Vector3 receivedPosition = Vector3.zero;
                stream.Serialize(ref receivedPosition);
                transform.position = receivedPosition;
            }
        }
    }
}
