//----------------------------------------------------------------------------
// <copyright file="RoughAssert.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace TestUtilities
{
    using NUnit.Framework;
    using UnityEngine;

    /// <summary>
    /// Testing utility class that provides rough equality checks for floating-point 
    /// based classes and structs. 
    /// <para>
    /// The methods in this class are designed to keep test case methods shorter and 
    /// more concise by providing rough equality testing for common Unity types.
    /// </para>
    /// </summary>
    public static class RoughAssert
    {
        /// <summary>
        /// Asserts that the expected <see cref="Vector3"/> is equal to the actual 
        /// <see cref="Vector3"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="Vector3"/>.</param>
        /// <param name="actual">The actual <see cref="Vector3"/>.</param>
        /// <param name="delta">The maximum difference of any one value in the <see cref="Vector3"/>.</param>
        public static void AreEqual(Vector3 expected, Vector3 actual, float delta)
        {
            Assert.AreEqual(expected.x, actual.x, delta);
            Assert.AreEqual(expected.y, actual.y, delta);
            Assert.AreEqual(expected.z, actual.z, delta);
        }

        /// <summary>
        /// Asserts that the expected <see cref="Quaternion"/> is equal to the actual
        /// <see cref="Quaternion"/>.
        /// <para>
        /// The <see cref="Quaternion"/> objects are compared according to their euler 
        /// representation, because there may be multiple <see cref="Quaternion"/> 
        /// instances that represent the same rotation but have different values
        /// (Most notably: <c>(0,0,0,1)</c> and <c>(0,0,0,0)</c> both represent the 
        /// identity rotation).
        /// </para>
        /// </summary>
        /// <param name="expected">The expected <see cref="Quaternion"/>.</param>
        /// <param name="actual">The actual <see cref="Quaternion"/>.</param>
        /// <param name="delta">The maximum difference (in degrees) of any rotation axis in the <see cref="Quaternion"/>.</param>
        public static void AreEqual(Quaternion expected, Quaternion actual, float delta)
        {
            Vector3 expectedAngles = expected.eulerAngles;
            Vector3 actualAngles = actual.eulerAngles;

            RotationEqual("x", expectedAngles.x, actualAngles.x, delta);
            RotationEqual("y", expectedAngles.y, actualAngles.y, delta);
            RotationEqual("z", expectedAngles.z, actualAngles.z, delta);
        }

        /// <summary>
        /// Tests whether the expected value is close enough to the actual value.
        /// <para>
        /// This method assumes <c>expected</c> and <c>actual</c> are rotations, and as such
        /// this method assumes that (for example) 359.9 and 0.0 are only 0.1 apart.
        /// </para>
        /// </summary>
        /// <param name="tag">A tag for the rotation axis to be evaluated. Used for constructing s useful failure message.</param>
        /// <param name="expected">The expected rotation, between 0 and 360.</param>
        /// <param name="actual">The actual rotation, between 0 and 360.</param>
        /// <param name="delta">The maximum difference in degrees between the two rotations.</param>
        public static void RotationEqual(string tag, float expected, float actual, float delta)
        {
            float diff = expected - actual;
            if (Mathf.Abs(diff) > delta && Mathf.Abs(diff - 360) > delta) 
            {
                Assert.Fail(
                    "Expected {3} Rotation: \t{0} +/- {1}\nBut got:\t\t{2}", 
                    expected, 
                    delta, 
                    actual, 
                    tag);
            }
        }
    }
}
