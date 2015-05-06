namespace Laser
{
	using System;
	using NUnit.Framework;
	using UnityEngine;

	[TestFixture]
    public class LaserTest
	{
		public static LaserEmitter CreateEmitter()
		{
			GameObject gameObject = new GameObject("Emitter", typeof(LaserEmitter));
			return gameObject.GetComponent<LaserEmitter>();
		}

		public static Laser CreateLaser(Vector3 origin, Vector3 direction)
		{
			Laser laser = new Laser(origin, direction, CreateEmitter());
			laser.Create();
			return laser;
		}

		[Test]
		public void ExtendTest()
		{
			Laser laser = CreateLaser(Vector3.zero, Vector3.forward);
			Laser extension = laser.Extend(Vector3.zero, Vector3.left);

			Assert.AreEqual(Vector3.zero, extension.Origin);
			Assert.AreEqual(Vector3.left, extension.Direction);
		}
	}
}

