namespace TestUtilities
{
    using System;
    using Core;
    using Core.Emitter;
    using Core.Receiver;
    using NUnit.Framework;
    using UnityEngine;

    /// <summary>
    /// A factory class which has some easy methods for constructing
    /// several game objects and returning their components.
    /// </summary>
    public class GameObjectFactory
    {
        /// <summary>
        /// Creates and returns an AND-gate game object.
        /// </summary>
        /// <returns>The AND-gate.</returns>
        public static ILaserReceiver CreateAndGate()
        {
            GameObject gameObject = new GameObject("ANDGate", typeof(AndGate));
            return gameObject.GetComponent<AndGate>();
        }

        /// <summary>
        /// Creates and returns an OR-gate game object.
        /// </summary>
        /// <returns>The OR-gate.</returns>
        public static ILaserReceiver CreateOrGate()
        {
            GameObject gameObject = new GameObject("ORGate", typeof(OrGate));
            return gameObject.GetComponent<OrGate>();
        }

        /// <summary>
        /// Creates and returns an emitter.
        /// </summary>
        /// <returns>The emitter.</returns>
        public static LaserEmitter CreateEmitter()
        {
            GameObject gameObject = new GameObject("Emitter", typeof(LaserEmitter));
            return gameObject.GetComponent<LaserEmitter>();
        }

        /// <summary>
        /// Creates and returns a laser beam
        /// </summary>
        /// <returns>The beam.</returns>
        public static LaserBeam CreateLaserBeam()
        {
            GameObject gameObject = new GameObject("Beam", typeof(LaserBeam));
            return gameObject.GetComponent<LaserBeam>();
        }
    }
}
