//----------------------------------------------------------------------------
// <copyright file="ShutdownBehaviour.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Behavior class that shuts down the application if a certain key is
/// pressed. Add this to any game object to make it work.
/// </summary>
public class ShutdownBehaviour : MonoBehaviour
{
    /// <summary>
    /// The key to press in order to quit the application.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public KeyCode QuitKey = KeyCode.Escape;

    /// <summary>
    /// Shuts down the application if the <c>QuitKey</c> is pressed.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(this.QuitKey))
        {
            Application.Quit();
        }
    }
}
