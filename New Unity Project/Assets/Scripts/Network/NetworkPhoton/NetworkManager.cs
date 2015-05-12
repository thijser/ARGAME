//----------------------------------------------------------------------------
// <copyright file="NetworkManager.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace NetworkPhoton
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Sets up a network using Photon.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        /// <summary>
        /// Sets up the network.
        /// </summary>
        public void Start()
        {
            PhotonNetwork.ConnectUsingSettings("0.1");
        }
    }
}
