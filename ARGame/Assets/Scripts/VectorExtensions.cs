﻿//----------------------------------------------------------------------------
// <copyright file="VectorExtensions.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using UnityEngine;

/// <summary>
/// Utility class with extension methods for the Vector3 class.
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    /// Get average value of the three vector components.
    /// </summary>
    /// <param name="vector">Vector component to use.</param>
    /// <returns>Average value of vector components.</returns>
    public static float Average(this Vector3 vector)
    {
        return (vector.x + vector.y + vector.z) / 3.0f;
    }

    /// <summary>
    /// Converts a <see cref="Vector3"/> to a <see cref="Vector4"/>
    /// suitable for use in linear transformations. 
    /// <para>
    /// The <c>x</c>, <c>y</c> and <c>z</c> components are set to 
    /// the values from the <see cref="Vector3"/>. The <c>w</c>
    /// component is set to 1.
    /// </para>
    /// </summary>
    /// <param name="vector">The <see cref="Vector3"/> to convert.</param>
    /// <returns></returns>
    public static Vector4 ToVec4(this Vector3 vector)
    {
        return new Vector4(vector.x, vector.y, vector.z, 1);
    }
}