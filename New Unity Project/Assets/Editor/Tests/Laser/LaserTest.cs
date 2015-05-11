//----------------------------------------------------------------------------
// <copyright file="LaserTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser
{
	using System;
	using NUnit.Framework;
	using UnityEngine;

	[TestFixture]
    public class LaserTest
	{
		/// <summary>
		/// Creates a LaserEmitter instance to create Laser segments with.
		/// </summary>
		/// <returns>The LaserEmitter.</returns>
		public static LaserEmitter CreateEmitter()
		{
			GameObject gameObject = new GameObject("Emitter", typeof(LaserEmitter));
			return gameObject.GetComponent<LaserEmitter>();
		}

		/// <summary>
		/// Creates a Laser beam with the given properties.
		/// </summary>
		/// <returns>The created Laser.</returns>
		/// <param name="origin">The origin.</param>
		/// <param name="direction">The direction.</param>
		public static Laser CreateLaser(Vector3 origin, Vector3 direction)
		{
			Laser laser = new Laser(origin, direction, CreateEmitter());
			laser.Create();
			return laser;
		}

		/// <summary>
		/// Tests the basic properties of the Laser and the <c>CreateLaser</c> method
		/// used by other test cases.
		/// </summary>
		[Test]
		public void CreateLaserTest()
		{
			Vector3 origin = new Vector3(0, 1, 2);
			Vector3 direction = new Vector3(3, 4, 5);
			Laser laser = CreateLaser(origin, direction);

			Assert.AreEqual(origin, laser.Origin);
			Assert.AreEqual(direction, laser.Direction);
		}

		/// <summary>
		/// Tests if the <c>Extend</c> method produces a Laser segment with the 
		/// correct properties.
		/// </summary>
		[Test]
		public void ExtendTest()
		{
			Laser laser = CreateLaser(Vector3.zero, Vector3.forward);
			Laser extension = laser.Extend(Vector3.zero, Vector3.left);

			Assert.AreEqual(Vector3.zero, extension.Origin);
			Assert.AreEqual(Vector3.left, extension.Direction);
			Assert.AreSame(laser.Emitter, extension.Emitter);
		}
	}
}

