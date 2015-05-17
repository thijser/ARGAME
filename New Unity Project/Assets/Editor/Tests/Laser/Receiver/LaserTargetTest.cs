namespace Core.Receiver
{
    using System;
    using Core.Emitter;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// A test class for the LaserTarget object.
    /// </summary>
    [TestFixture]
    public class LaserTargetTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the laser target is not opening when 
        /// no laser hits it.
        /// </summary>
        [Test]
        public static void NoLaserHitTest()
        {
            LaserTarget lt = (LaserTarget) CreateLaserTarget();
            Assert.False(lt.IsOpening);
        }

        /// <summary>
        /// Tests if crystal is opening when a valid laser beam hit it.
        /// </summary>
        [Test]
        public static void LaserHitTest()
        {
            GameObject gameObject = new GameObject("LaserTarget", typeof(LaserTarget));
            gameObject.AddComponent<Animator>();
            gameObject.GetComponent<LaserTarget>().OnLaserHit(null, new HitEventArgs(CreateTestBeam(), Vector3.zero, Vector3.forward, CreateAndGate()));
            gameObject.GetComponent<LaserTarget>().LateUpdate();
            Assert.True(gameObject.GetComponent<LaserTarget>().IsOpening);
        }
    }
}
