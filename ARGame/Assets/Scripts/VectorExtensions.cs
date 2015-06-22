using UnityEngine;

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
}