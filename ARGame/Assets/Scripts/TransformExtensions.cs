//----------------------------------------------------------------------------
// <copyright file="TransformExtensions.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
//
//     
//     Some of the methods in the TransformExtensions class have been retrieved 
//     from the Unity forums. See the following web page for the original post:
//     http://forum.unity3d.com/threads/how-to-assign-matrix4x4-to-transform.121966/#post-1830992
// </copyright>
//----------------------------------------------------------------------------
using UnityEngine;

/// <summary>
/// Extension methods for the Transform class.
/// <para>
/// This class adds functionality to set the fields in the transform
/// based on a Matrix4x4 which is the result of a 3D transformation.
/// </para>
/// <para>
/// The implementation of the <c>ExtractXXX</c> methods is derived from a 
/// thread on the Unity forums. See the following link for the forum post:
/// http://forum.unity3d.com/threads/how-to-assign-matrix4x4-to-transform.121966/#post-1830992
/// </para>
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// Sets the position, rotation and scale from the given Matrix.
    /// <para>
    /// If the Matrix does not represent a regular 3D transformation matrix, the
    /// resulting state of the transform is undefined.
    /// </para>
    /// </summary>
    /// <param name="transform">The Transform to apply the Matrix to, not null.</param>
    /// <param name="matrix">The Matrix to apply, not null</param>
    public static void SetFromMatrix(this Transform transform, Matrix4x4 matrix)
    {
        transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        transform.localScale = ExtractScaleFromMatrix(ref matrix);
    }

    /// <summary>
    /// Logs the position, rotation and scale of the Transform marked with the provided label
    /// to the error stream.
    /// <para>
    /// This method is purely intended for debugging purposes, as it will have little purpose 
    /// in other situations.
    /// </para>
    /// </summary>
    /// <param name="transform">The transform to log, not null.</param>
    /// <param name="label">The label to apply to the transform.</param>
    public static void LogAs(this Transform transform, string label)
    {
        Debug.LogError(label + ": " + transform.position.ToString("G4")
            + ", " + transform.eulerAngles.ToString("G4")
            + ", " + transform.lossyScale.ToString("G4"));
    }

    /// <summary>
    /// Extracts the highest parent from this transform.
    /// </summary>
    /// <param name="t">The transform to extract the parent from.</param>
    /// <returns>The highest parent.</returns>
    public static Transform GetHighestParent(this Transform t)
    {
        Transform t1 = t;
        while (t1.parent != null)
        {
            t1 = t1.parent;
        }

        return t1;
    }

    /// <summary>
    /// Extract translation from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Translation offset.
    /// </returns>
    private static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 translate;
        translate.x = matrix.m03;
        translate.y = matrix.m13;
        translate.z = matrix.m23;
        return translate;
    }

    /// <summary>
    /// Extract rotation quaternion from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Quaternion representation of rotation transform.
    /// </returns>
    private static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    /// <summary>
    /// Extract scale from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Scale vector.
    /// </returns>
    private static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
}