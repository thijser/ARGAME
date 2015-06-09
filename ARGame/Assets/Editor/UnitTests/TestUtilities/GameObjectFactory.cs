//----------------------------------------------------------------------------
// <copyright file="GameObjectFactory.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace TestUtilities
{
    using Core;
    using Core.Emitter;
    using Core.Receiver;
    using Network;
    using Projection;
    using Graphics;
    using UnityEngine;

    /// <summary>
    /// A factory class which has some easy methods for constructing
    /// several game objects and returning their components.
    /// </summary>
    public class GameObjectFactory
    {
        /// <summary>
        /// Creates and returns a LaserTarget game object.
        /// </summary>
        /// <returns>The laser target.</returns>
        public static LaserTarget CreateLaserTarget()
        {
            GameObject gameObject = new GameObject("LaserTarget", typeof(LaserTarget));
            gameObject.AddComponent<Animator>();
            return gameObject.GetComponent<LaserTarget>();
        }

        /// <summary>
        /// Creates and returns an emitter.
        /// </summary>
        /// <returns>The emitter.</returns>
        public static LaserEmitter CreateEmitter()
        {
            GameObject gameObject = new GameObject(
                "Emitter",
                typeof(LaserEmitter),
                typeof(VolumeLineRenderer),
                typeof(LaserProperties));
            LaserEmitter emitter = gameObject.GetComponent<LaserEmitter>();
            emitter.LineRenderer = gameObject.GetComponent<VolumeLineRenderer>();
            return emitter;
        }

        /// <summary>
        /// Creates and returns a multi emitter.
        /// </summary>
        /// <returns>The emitter.</returns>
        public static MultiEmitter CreateMultiEmitter()
        {
            GameObject gameObject = new GameObject(
                "MultiEmitter",
                typeof(MultiEmitter),
                typeof(LaserEmitter));
            return gameObject.GetComponent<MultiEmitter>();
        }

        /// <summary>
        /// Creates and returns a laser beam.
        /// </summary>
        /// <returns>The laser beam.</returns>
        public static LaserBeam CreateTestBeam()
        {
            return new LaserBeam(Vector3.zero, Vector3.forward, CreateEmitter());
        }

        /// <summary>
        /// Creates a valid HitEventArgs object for testing purposes.
        /// </summary>
        /// <returns>A valid HitEventArgs object.</returns>
        public static HitEventArgs CreateTestHit()
        {
            return new HitEventArgs(CreateTestBeam(), Vector3.zero, Vector3.forward, Create<AndGate>());
        }

        /// <summary>
        /// Creates a portal that is linked to another portal.
        /// </summary>
        /// <returns>A linked portal.</returns>
        public static Portal CreateLinkedPortal()
        {
            GameObject gameObject = new GameObject("Portal", typeof(Portal));
            GameObject gameObject2 = new GameObject("Portal2", typeof(Portal));
            Portal portal1 = gameObject.GetComponent<Portal>();
            portal1.LinkedPortal = gameObject2.GetComponent<Portal>();
            return portal1;
        }

        /// <summary>
        /// Creates a GameObject that has the single given Component type.
        /// </summary>
        /// <typeparam name="T">The Component Type</typeparam>
        /// <returns>The created Component</returns>
        public static T Create<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            GameObject gameObject = new GameObject(type.Name, type);
            return gameObject.GetComponent<T>();
        }
    }
}
