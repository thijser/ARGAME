//----------------------------------------------------------------------------
// <copyright file="ArrowKeyMove.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Allows moving the GameObject with the arrow keys.
/// </summary>
public class ArrowKeyMove : MonoBehaviour
{
    /// <summary>
    /// The speed with which to move the mirror.
    /// </summary>
    public const float Speed = 10f;

    /// <summary>
    /// Processes user input.
    /// </summary>
    public void Update()
    {
        this.InputMovement();
    }

    /// <summary>
    /// Processes user input.
    /// </summary>
    public void InputMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.GetComponent<Transform>().position = this.GetComponent<Transform>().position + (Vector3.forward * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.GetComponent<Transform>().position = this.GetComponent<Transform>().position - (Vector3.forward * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.GetComponent<Transform>().position = this.GetComponent<Transform>().position + (Vector3.right * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.GetComponent<Transform>().position = this.GetComponent<Transform>().position - (Vector3.right * Speed * Time.deltaTime);
        }
    }
}
