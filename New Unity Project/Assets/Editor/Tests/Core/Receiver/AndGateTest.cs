//----------------------------------------------------------------------------
// <copyright file="AndGateTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Receiver
{
    using System;
    using Core.Emitter;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// A test class for the LaserTarget object.
    /// </summary>
    [TestFixture]
    public class AndGateTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if a MultiEmitter object is added when 
        /// the object is instantiated in Unity.
        /// </summary>
        [Test]
        public static void StartTest()
        {
            AndGate ag = CreateAndGate();
            Assert.True(ag.PassThroughEmitter == null);
            ag.Start();
            Assert.False(ag.PassThroughEmitter == null);
        }

        /// <summary>
        /// Tests if the other variables 
        /// are correctly instantiated.
        /// </summary>
        [Test]
        public static void StartTest2()
        {
            AndGate ag = CreateAndGate();
            ag.Start();
            Assert.False(ag.Hit);
            Assert.False(ag.BeamCreated);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// an invalid HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidOnLaserHit()
        {
            AndGate ag = CreateAndGate();
            ag.OnLaserHit(null, new HitEventArgs());
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null reference HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullOnLaserHit()
        {
            AndGate ag = CreateAndGate();
            ag.OnLaserHit(null, null);
        }

        /// <summary>
        /// Tests if the hit flag is set to true,
        /// but no beam is created, when one laser hits
        /// the gate.
        /// </summary>
        [Test]
        public void OneLaserHit()
        {
            AndGate ag = CreateAndGate();
            ag.OnLaserHit(null, MirrorsUnitTest.CreateTestHit());
            Assert.True(ag.Hit);
            Assert.False(ag.BeamCreated);
        }

        /// <summary>
        /// Tests if the hit flag is set to true,
        /// and a beam is created, when more lasers hit
        /// the gate.
        /// </summary>
        [Test]
        public void MoreLaserHit()
        {
            AndGate ag = CreateAndGate();
            ag.OnLaserHit(null, MirrorsUnitTest.CreateTestHit());
            ag.OnLaserHit(null, MirrorsUnitTest.CreateTestHit());
            Assert.True(ag.Hit);
            Assert.True(ag.BeamCreated);
        }
    }
}