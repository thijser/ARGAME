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
            new HitEventArgs(new LaserBeam(Vector3.zero, Vector3.zero, CreateEmitter()), Vector3.zero, Vector3.zero, null);
        }

        /// <summary>
        /// Tests if the constructor is functioning correctly
        /// when the parameters are non-null.
        /// </summary>
        [Test]
        public static void ConstructorNoArgNull()
        {
            LaserBeam lb = new LaserBeam(Vector3.zero, Vector3.zero, CreateEmitter());
            ILaserReceiver ilr = CreateOrGate();
            Vector3 zero1 = Vector3.zero;
            Vector3 zero2 = Vector3.zero;
            HitEventArgs hea = new HitEventArgs(lb, zero1, zero2, ilr);
            Assert.True(hea.IsValid);
            Assert.True(lb == hea.Laser);
            Assert.True(ilr == hea.Receiver);
            Assert.True(hea.Point == zero1);
            Assert.True(hea.Normal == zero2);
        }

        /// <summary>
        /// Tests if the invalid constructor is functioning correctly.
        /// </summary>
        [Test]
        public static void ConstructorInvalid()
        {
            HitEventArgs hea = new HitEventArgs();
            Assert.False(hea.IsValid);
        }
    }
}
