//----------------------------------------------------------------------------
// <copyright file="ShutdownBehaviour.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using UnityEngine;

/// <summary>
/// Behavior class that shuts down the application if the escape key is
/// pressed. Add this to any game object to make it work.
/// </summary>
public class ShutdownBehaviour : MonoBehaviour
{
    /// <summary>
    /// Shuts down the application if the escape key is pressed.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
