//----------------------------------------------------------------------------
// <copyright file="PlayerColour.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using UnityEngine;

/// <summary>
/// Updates the colors of player heads.
/// </summary>
public class PlayerColour : MonoBehaviour
{
    /// <summary>
    /// The color of the player.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Sets the color of the player head.
    /// </summary>
    public void Start()
    {
        this.SetColor(this.Color);
    }

    /// <summary>
    /// Sets the color of the player head to the given <see cref="Color"/>.
    /// </summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    public void SetColor(Color color)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.material.color = color;
        renderer.material.SetColor(0, color);
        renderer.material.SetColor(1, color);
        renderer.material.SetColor(2, color);
        renderer.material.SetColor(3, color);
        renderer.material.SetColor(4, color);
        renderer.material.SetColor(5, color);
    }
}
