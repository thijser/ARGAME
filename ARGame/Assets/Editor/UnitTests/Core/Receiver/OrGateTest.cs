﻿//----------------------------------------------------------------------------
// <copyright file="OrGateTest.cs" company="Delft University of Technology">
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
    /// A test class for the OrGate object.
    /// </summary>
    [TestFixture]
    public class OrGateTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if a MultiEmitter object is added when 
        /// the object is instantiated in Unity.
        /// </summary>
        [Test]
        public void StartTest()
        {
            OrGate og = Create<OrGate>();
            Assert.True(og.PassThroughEmitter == null);
            og.Start();
            Assert.False(og.PassThroughEmitter == null);
        }

        /// <summary>
        /// Tests if the other variables 
        /// are correctly instantiated.
        /// </summary>
        [Test]
        public void StartTest2()
        {
            OrGate og = Create<OrGate>();
            og.Start();
            Assert.False(og.BeamCreated);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// an invalid HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidOnLaserHit()
        {
            OrGate og = Create<OrGate>();
            og.OnLaserHit(null, new HitEventArgs());
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null reference HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullOnLaserHit()
        {
            OrGate og = Create<OrGate>();
            og.OnLaserHit(null, null);
        }

        /// <summary>
        /// Tests if a beam is created, when one laser hits
        /// the gate.
        /// </summary>
        [Test]
        public void OneLaserHit()
        {
            OrGate og = Create<OrGate>();
            og.Start();
            og.OnLaserHit(null, MirrorsUnitTest.CreateTestHit());
            Assert.True(og.BeamCreated);
        }
    }
}