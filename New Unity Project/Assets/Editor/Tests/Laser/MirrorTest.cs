namespace Laser
{
	using System;
    using System.Collections.ObjectModel;
	using NUnit.Framework;
	using UnityEngine;

    /// <summary>
    /// Unit test class for the <see cref="Mirror"/> class.
    /// </summary>
    [TestFixture]
    internal class MirrorTest
    {
		/// <summary>
		/// Creates a Mirror with the specified position and rotation.
		/// </summary>
		/// <returns>The created Mirror.</returns>
		/// <param name="position">the position.</param>
		/// <param name="rotation">the rotation.</param>
		public static Mirror CreateMirror(Vector3 position, Vector3 rotation)
		{
			GameObject obj = new GameObject("TestMirror", typeof(Mirror));
			Mirror m = obj.GetComponent<Mirror>();
			obj.transform.position = position;
			obj.transform.eulerAngles = rotation;
			return m;
		}

		/// <summary>
		/// Tests if calling <c>OnLaserHit(this, null)</c> throws an
		/// ArgumentNullException.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullTest() 
		{
			new Mirror().OnLaserHit(this, null);
		}

		/// <summary>
		/// Tests if a perpendicular laser beam is reflected in the opposite direction.
		/// </summary>
		[Test]
		public void PerpendicularTest()
		{
			GameObject obj = new GameObject("TestEmitter", typeof(LaserEmitter));
			LaserEmitter emitter = obj.GetComponent<LaserEmitter>();
			Laser l = new Laser (Vector3.zero, Vector3.zero, emitter);
			HitEventArgs args = new HitEventArgs(l, Vector3.zero);
			Mirror m = CreateMirror(Vector3.zero, new Vector3 (0, 90, 0));
			m.OnLaserHit(this, args);
			ReadOnlyCollection<Laser> lasers = emitter.GetLasers();
			Assert.AreEqual(2, lasers.Count);
			Laser created = lasers[1];
			Assert.AreEqual(new Vector3(0, 180, 0), created.Direction);
		}
    }
}