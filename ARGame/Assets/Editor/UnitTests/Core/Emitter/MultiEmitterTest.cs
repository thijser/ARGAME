//----------------------------------------------------------------------------
// <copyright file="MultiEmitterTest.cs" company="Delft University of Technology">
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
    /// A test class for the multi emitter.
    /// </summary>
    [TestFixture]
    public class MultiEmitterTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the correct exception is thrown when 
        /// ApplyProperties is called with a null reference.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplyTest()
        {
            MultiEmitter.ApplyProperties(null, GameObjectFactory.CreateTestBeam());
        }

        /// <summary>
        /// Tests if the correct exception is thrown when 
        /// ApplyProperties is called with a null reference.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplyTest2()
        {
            MultiEmitter.ApplyProperties(CreateEmitter().GetComponent<LineRenderer>(), null);
        }

        /// <summary>
        /// Tests if the result of a valid call
        /// to ApplyProperties is not null.
        /// </summary>
        [Test]
        public void ApplyTestValid()
        {
            Assert.NotNull(MultiEmitter.ApplyProperties(CreateEmitter().GetComponent<LineRenderer>(), GameObjectFactory.CreateTestBeam()));
        }

        /// <summary>
        /// Tests if the enabling and disabling
        /// of lasers functions as it should.
        /// </summary>
        [Test]
        public void EnableDisableTest()
        {
            MultiEmitter me = CreateMultiEmitter();
            LaserEmitter le = me.GetEmitter(CreateTestBeam());
            Assert.True(le.Enabled);
            me.DisableAll();
            Assert.False(le.Enabled);
        }
    }
}