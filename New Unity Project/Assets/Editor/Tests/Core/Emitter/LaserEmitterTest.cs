//----------------------------------------------------------------------------
// <copyright file="LaserEmitterTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Emitter
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// A test class for the lens splitter.
    /// </summary>
    [TestFixture]
    public class LaserEmitterTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the object does not
        /// raise its hit flag without being hit.
        /// </summary>
        [Test]
        public void StartTest()
        {
            LaserEmitter le = CreateEmitter();
            le.Start();
            Assert.True(le.Enabled);
            Assert.NotNull(le.LineRenderer);
        }

        /// <summary>
        /// Tests if adding a laser beam happens correctly.
        /// </summary>
        [Test]
        public void AddLaserBeamTest()
        {
            LaserEmitter le = CreateEmitter();
            le.Start();
            int i = le.Segments.Count;
            le.AddLaser(CreateTestBeam());
            Assert.True(le.Segments.Count == i + 1);
        }

        /// <summary>
        /// Tests if clearing the beams happens correctly.
        /// </summary>
        [Test]
        public void ClearLaserBeamTest()
        {
            LaserEmitter le = CreateEmitter();
            le.Start();
            le.AddLaser(CreateTestBeam());
            le.Clear();
            Assert.True(le.Segments.Count == 0);
        }

        /// <summary>
        /// Tests if updating executes correctly.
        /// </summary>
        [Test]
        public void UpdateTestEnabled()
        {
            LaserEmitter le = CreateEmitter();
            le.Start();
            le.AddLaser(CreateTestBeam());
            le.Update();
            Assert.False(le.Segments.Count == 0);
        }
    }
}