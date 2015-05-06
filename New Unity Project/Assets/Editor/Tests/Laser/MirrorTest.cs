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
		/// Creates a Mirror GameObject.
		/// </summary>
		/// <returns>The created Mirror</returns>
		public static Mirror CreateMirror()
		{
			GameObject obj = new GameObject ("Mirror", typeof(Mirror));
			return obj.GetComponent<Mirror> ();
		}

		/// <summary>
		/// Tests if calling <c>OnLaserHit(this, null)</c> throws an
		/// ArgumentNullException.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullHitArgsTest() 
		{
			CreateMirror().OnLaserHit(this, null);
		}

		///	<summary>
		/// Tests if calling <c>CreateReflection(null, Vector3.zero)</c>
		/// throws an ArgumentNullException.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullLaserBeamTest() 
		{
			CreateMirror().CreateReflection(null, Vector3.zero);
		}
    }
}