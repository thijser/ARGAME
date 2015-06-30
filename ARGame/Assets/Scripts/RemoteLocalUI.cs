//----------------------------------------------------------------------------
// <copyright file="LaserEmitter.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class RemoteLocalUI : MonoBehaviour
{
    string ip = "";

    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 150, 90), "Loader Menu");
        GUI.Box(new Rect(10, 110, 150, 90), "Enter IP");

        ip = GUI.TextField(new Rect(20, 130, 130, 60), ip);

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(20, 40, 130, 20), "Local"))
        {
            Application.LoadLevel(1);
        }

        // Make the second button.
        if (GUI.Button(new Rect(20, 70, 130, 20), "Remote"))
        {
            Application.LoadLevel(2);
        }
    }
}