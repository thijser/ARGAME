//----------------------------------------------------------------------------
// <copyright file="MirrorsUnitTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace TestUtilities
{
    using System;
    using NUnit.Framework;
    using UnityEngine;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// Base class for unit tests for the Mirrors AR game.
    /// <para>
    /// This class takes care of providing a clean environment to run the test cases in,
    /// as by default, unit tests are run in the current scene (which may affect the 
    /// test outcome, depending on the contents of the scene).
    /// </para>
    /// </summary>
    public class MirrorsUnitTest
    {
        /// <summary>
        /// Tests whether the object is non-interfering. That is, whether the object 
        /// has any impact on the test execution.
        /// <para>
        /// If this method returns true for a GameObject, it will not be removed from 
        /// the scene after a test. Unit test classes can override this method to 
        /// accommodate for custom scenarios as required.
        /// </para>
        /// <para>
        /// It is advised, but not required, that overriding methods call their base 
        /// implementation first.
        /// </para>
        /// <para>
        /// The default implementation excludes the Main Camera and Directional Light 
        /// (present in an empty scene) from being removed.</para>
        /// </summary>
        /// <param name="obj">The GameObject to check, not null.</param>
        /// <returns>True if the GameObject does not interfere with the test execution, false otherwise.</returns>
        public virtual bool IsNonInterfering(GameObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.name == "Main Camera" || obj.name == "Directional Light";
        }

        /// <summary>
        /// Verifies the scene is empty before running the test. 
        /// Fails the test otherwise.
        /// </summary>
        [SetUp]
        public virtual void SetUpTest()
        {
            if (Array.Exists(
                    UnityObject.FindObjectsOfType<GameObject>(), 
                    e => !this.IsNonInterfering(e)))
            {
                Debug.LogError("Unit tests are being run in a non-empty scene. Clear the scene and try again.");
                Assert.Fail("Failed because current scene is not empty.");
            }
        }

        /// <summary>
        /// Clears the scene after running the test.
        /// </summary>
        [TearDown]
        public virtual void TearDownTest()
        {
            foreach (GameObject obj in UnityObject.FindObjectsOfType<GameObject>())
            {
                if (!this.IsNonInterfering(obj))
                {
                    GameObject.DestroyImmediate(obj);
                }
            }
        }
    }
}
