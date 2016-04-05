//----------------------------------------------------------------------------
// <copyright file="RemoteLocalUI.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides a user interface where the user can select to play as local or remote 
/// player and can enter a server IP address.
/// </summary>
public class RemoteLocalUI : MonoBehaviour
{
    /// <summary>
    /// The server IP address to connect to.
    /// </summary>
    public static string IPAddress = string.Empty;

    /// <summary>
    /// The last error that occurred. 
    /// <para>
    /// If no error occurred, this value is null.
    /// </para>
    /// </summary>
    private string errorMessage = null;

    /// <summary>
    /// Shows the user interface.
    /// </summary>
    public void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(Screen.width / 2 - 190, Screen.height / 2 - 120, 150, 90), "Loader Menu");
        GUI.Box(new Rect(Screen.width / 2 - 190, Screen.height / 2 - 20, 150, 90), "Enter Host");

        if (!string.IsNullOrEmpty(this.errorMessage))
        {
            GUI.Box(new Rect(Screen.width / 2 - 20, Screen.height / 2 - 120, 200, 190), this.errorMessage);
        }

        RemoteLocalUI.IPAddress = GUI.TextField(new Rect(Screen.width / 2 - 180, Screen.height / 2, 130, 60), RemoteLocalUI.IPAddress);

        if (GUI.Button(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 90, 130, 20), "Local"))
        {
            this.TryLoadNext(RemoteLocalUI.IPAddress, 1);
        }

        if (GUI.Button(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 60, 130, 20), "Remote"))
        {
            this.TryLoadNext(RemoteLocalUI.IPAddress, 2);
        }
    }

    /// <summary>
    /// Tries to load the scene indicated by the given index.
    /// <para>
    /// This method first validates the server address and returns false
    /// when the address is invalid. Otherwise, this method loads the 
    /// scene at the given index and returns true.
    /// </para>
    /// </summary>
    /// <param name="ipAddress">The server IP to connect to.</param>
    /// <param name="index">The scene index to load.</param>
    /// <returns>True if connecting to the server succeeded, false otherwise.</returns>
    public bool TryLoadNext(string ipAddress, int index)
    {
        bool isValid = this.CheckIPValid(ipAddress);
        if (isValid)
        {
            SceneManager.LoadScene(index);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the given host address is valid as an IP address.
    /// </summary>
    /// <param name="address">The host address to connect to.</param>
    /// <returns>True if it is valid.</returns>
    public bool CheckIPValid(string address)
    {
        try
        {
            IPAddress[] addresses = Dns.GetHostAddresses(address);
            if (addresses.Length == 0)
            {
                return false;
            }

            RemoteLocalUI.IPAddress = addresses[0].ToString();
            return true;
        }
        catch (SocketException ex)
        {
            this.errorMessage = "Could not connect \n to the server at '" + address + "':\n " + ex.Message;
            return false;
        }
    }
}