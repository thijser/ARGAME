//----------------------------------------------------------------------------
// <copyright file="PositionLinker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Describes modes for linking position and rotation.
/// </summary>
public enum LinkingMode
{
    /// <summary>
    /// Follow the exact position and rotation.
    /// </summary>
    Exact = 0,

    /// <summary>
    /// Ignore the y-rotation.
    /// </summary>
    IgnoreHeight = 1,

    /// <summary>
    /// Synchronize the position only, and ignore the rotation.
    /// </summary>
    PositionOnly = 2,

    /// <summary>
    /// Follow the rotation of the level marker.
    /// </summary>
    FollowLevel = 3,

    /// <summary>
    /// Project position to plane of level.
    /// </summary>
    Project = 4
}

/// <summary>
/// Links to a Transform and keeps it updated as this game object's Transform changes.
/// </summary>
public class PositionLinker : MonoBehaviour
{
    /// <summary>
    /// The Transform this PositionLinker is linked to.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public Transform LinkedTo;

    /// <summary>
    /// The LinkingMode to use.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public LinkingMode Mode = LinkingMode.FollowLevel;

    /// <summary>
    /// The Transform of the level marker.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
    public Transform LevelMarker;

    /// <summary>
    /// The angle of the LinkedTo Transform.
    /// </summary>
    private float angle = 0;

    public void Linked()
    {
        if (this.Mode == LinkingMode.Project)
        {
            Vector3 v1 = transform.position - this.LevelMarker.position;
            Vector3 n = this.LevelMarker.up;

            Vector3 proj = v1 - (Vector3.Dot(v1, n) * n);
            proj += this.LevelMarker.position;

            this.LinkedTo.position = proj;
        }
        else
        {
            this.LinkedTo.position = transform.position;
        }

        switch (this.Mode)
        {
            case LinkingMode.Exact:
                this.LinkedTo.rotation = transform.rotation;
                break;
            case LinkingMode.IgnoreHeight:
                Vector3 angles = transform.rotation.eulerAngles;
                angles.x = this.LinkedTo.eulerAngles.x;
                angles.z = this.LinkedTo.eulerAngles.z;
                this.LinkedTo.rotation = Quaternion.Euler(angles);
                break;
            case LinkingMode.PositionOnly:
                break;
            case LinkingMode.FollowLevel:
                this.LinkedTo.rotation = this.LevelMarker.rotation;
                break;
            case LinkingMode.Project:
                this.LinkedTo.rotation = this.LevelMarker.rotation;
                this.LinkedTo.rotation *= Quaternion.AngleAxis(this.angle, new Vector3(0, 1, 0));
                break;
            default:
                throw new ArgumentException("Invalid LinkingMode");
        }

    }

	/// <summary>
	/// Updates the position and/or rotation of the LinkedTo Transform.
	/// </summary>
    public void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.angle += 1.0f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.angle -= 1.0f;
        }

    }
}
