//----------------------------------------------------------------------------
// <copyright file="PrintLocation.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using UnityEngine;

/// <summary>
/// Debug script to print the location and rotation of a game object.
/// </summary>
public class PrintLocation : MonoBehaviour 
{
    /// <summary>
    /// Prints the position and rotation of the game object.
    /// </summary>
    public void Update() 
    {
        Debug.Log("position: "  + transform.position);
        Debug.Log("rotation: " + transform.rotation);
    }
}
