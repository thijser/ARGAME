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
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="LaserBeam.LaserBeam"/> class.
    /// </summary>
    [TestFixture]
    public class LaserBeamTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests the basic properties of the Laser and the <c>CreateLaser</c> method
        /// used by other test cases.
        /// </summary>
        [Test]
        public void CreateLaserTest()
        {
            Vector3 origin = new Vector3(0, 1, 2);
            Vector3 direction = new Vector3(3, 4, 5);
            LaserBeam laser = CreateLaser(origin, direction);

            Assert.AreEqual(origin, laser.Origin);
            Assert.AreEqual(direction, laser.Direction);
        }
    }
}