//----------------------------------------------------------------------------
// <copyright file="PositionUpdateTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Test class for PositionPreviewer.
    /// </summary>
    [TestFixture]
    public class PositionUpdateTest : MirrorsUnitTest
    {
        /// <summary>
        /// Several hash code tests, as there is very little to test in this class.
        /// </summary>
        [Test]
        public void HashTest()
        {
            PositionUpdate pu = new PositionUpdate(UpdateType.UpdatePosition, new Vector2(0, 0), 0f, 0);
            Assert.True(pu.GetHashCode() == 0);
            pu = new PositionUpdate(UpdateType.DeletePosition, new Vector2(0, 0), 0f, 0);
            Assert.True(pu.GetHashCode() == 125);
        }
    }
}
