//----------------------------------------------------------------------------
// <copyright file="HitEventArgsTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using System;
    using Core.Receiver;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Test class for the HitEventArgs class. 
    /// All options are tested to test the maximum amount of code.
    /// </summary>
    [TestFixture]
    public class HitEventArgsTest : MirrorsUnitTest
    {

        /// <summary>
        /// Tests if the correct exception is thrown when the 
        /// first argument is null.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public static void ConstructorFirstArgNull()
        {
            new HitEventArgs(null, Vector3.zero, Vector3.zero, CreateAndGate());
        }

        /// <summary>
        /// Tests if the correct exception is thrown when the 
        /// last argument is null.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public static void ConstructorLastArgNull()
        {
            new HitEventArgs(CreateLaserBeam(), Vector3.zero, Vector3.zero, null);
        }
    }
}
