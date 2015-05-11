//----------------------------------------------------------------------------
// <copyright file="ComponentUtilities.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Static utility class that adds some extension methods for working with Components.
/// </summary>
public static class ComponentUtilities 
{
    /// <summary>
    /// Copies a Component to another GameObject using reflection calls.
    /// <para>
    /// Note that not all Unity Components are fully copied, as some Component 
    /// types may have complex inner data structures or weak references to 
    /// other objects. In such cases, the copy may not be a deep copy.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the Component to copy.</typeparam>
    /// <param name="target">The target GameObject, not null.</param>
    /// <param name="component">The Component to copy, not null.</param>
    /// <returns>The copied Component.</returns>
    public static T AddComponentAsCopy<T>(this GameObject target, T component) where T : Component
    {
        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        if (component == null)
        {
            throw new ArgumentNullException("component");
        }

        Type type = component.GetType();
        Component copy = target.AddComponent(type);
        FieldInfo[] fields = type.GetFields();
        foreach (var field in fields)
        {
            field.SetValue(copy, field.GetValue(component));
        }

        return copy as T;
    }
}
