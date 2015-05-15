//----------------------------------------------------------------------------
// <copyright file="SimplePathBuilderTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace RandomLevel
{
    using System;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="SimplePathBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class SimplePathBuilderTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if passing null into the SimplePathBuilder constructor throws 
        /// an ArgumentNullException.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPassNullIntoConstructor()
        {
            new SimplePathBuilder(null);
        }
    }
}