//----------------------------------------------------------------------------
// <copyright file="LaserEmitter.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System;
using System.Collections;
using System.Net;
using Network;
using UnityEngine;
using System.Net.Sockets;

/// <summary>
/// Small script that creates a UI useful for selecting between being a local or remote player.
/// </summary>
public class RemoteLocalUI : MonoBehaviour
{
    public static string ip = "";
    private bool enteredWronglyOnce = false;
    private bool invalid = false;

    public void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 150, 90), "Loader Menu");
        GUI.Box(new Rect(10, 110, 150, 90), "Enter Host");
        if (enteredWronglyOnce)
        {
            GUI.Box(new Rect(170, 10, 200, 190), "Please enter a valid host.");
        }

        ip = GUI.TextField(new Rect(20, 130, 130, 60), ip);

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(20, 40, 130, 20), "Local"))
        {
            invalid = !CheckIPValid(ip);
            if (invalid)
            {
                enteredWronglyOnce = true;
                return;
            }
            else
            {
                Application.LoadLevel(1);
            }
        }

        // Make the second button.
        if (GUI.Button(new Rect(20, 70, 130, 20), "Remote"))
        {
            invalid = !CheckIPValid(ip);
            if (invalid)
            {
                enteredWronglyOnce = true;
                return;
            }
            else
            {
                Application.LoadLevel(2);
            }
        }
    }

    public bool CheckIPValid(String strIP)
    {
        try
        {
            IPAddress[] addresses = Dns.GetHostEntry(strIP).AddressList;
            if (addresses.Length == 0)
            {
                return false;
            }

            ip = addresses[0].ToString();

            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }
}