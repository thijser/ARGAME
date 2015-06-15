//----------------------------------------------------------------------------
// <copyright file="LaserBeamTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using Core.Emitter;
    using Core.Receiver;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="LaserBeam.LaserBeam"/> class.
    /// </summary>
    [TestFixture]
    public class WinConditionTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests the basic properties of the win condition.
        /// </summary>
        [Test]
        public void NotStartTest()
        {
            WinCondition cond = GameObjectFactory.Create<WinCondition>();
            Assert.Null(cond.Targets);
            Assert.Null(cond.Checks);
        }

        /// <summary>
        /// Tests the basic properties of the win condition after starting the class.
        /// </summary>
        [Test]
        public void StartTest()
        {
            WinCondition cond = GameObjectFactory.Create<WinCondition>();
            cond.Start();
            Assert.NotNull(cond.Targets);
            Assert.NotNull(cond.Checks);
            Assert.True(cond.Targets.Length == 0);
            Assert.True(cond.Checks.Length == 0);
        }

        /// <summary>
        /// Tests the basic properties of the win condition after starting the class,
        /// while the scene has instantiated targets and checkpoints.
        /// </summary>
        [Test]
        public void StartTestNotEmpty()
        {
            WinCondition cond = GameObjectFactory.Create<WinCondition>();
            LaserTarget lt = Create<LaserTarget>();
            Checkpoint check = Create<Checkpoint>();
            cond.Start();
            Assert.NotNull(cond.Targets);
            Assert.NotNull(cond.Checks);
            Assert.True(cond.Targets.Length > 0);
            Assert.True(cond.Checks.Length > 0);
        }
    }
}