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
        public static AndGate CreateAndGate()
        {
            GameObject gameObject = new GameObject("ANDGate", typeof(AndGate));
            return gameObject.GetComponent<AndGate>();
        }

        /// <summary>
        /// Creates and returns an OR-gate game object.
        /// </summary>
        /// <returns>The OR-gate.</returns>
        public static OrGate CreateOrGate()
        {
            GameObject gameObject = new GameObject("ORGate", typeof(OrGate));
            return gameObject.GetComponent<OrGate>();
        }

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
                typeof(LineRenderer),
                typeof(LaserProperties));
            LaserEmitter emitter = gameObject.GetComponent<LaserEmitter>();
            emitter.LineRenderer = gameObject.GetComponent<LineRenderer>();
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
            return new HitEventArgs(CreateTestBeam(), Vector3.zero, Vector3.forward, CreateAndGate());
        }

        /// <summary>
        /// Creates and returns a beam splitter game object.
        /// </summary>
        /// <returns>The beam splitter.</returns>
        public static BeamSplitter CreateBeamSplitter()
        {
            GameObject gameObject = new GameObject("Splitter", typeof(BeamSplitter));
            return gameObject.GetComponent<BeamSplitter>();
        }

        /// <summary>
        /// Creates and returns a lens splitter game object.
        /// </summary>
        /// <returns>The lens splitter.</returns>
        public static LensSplitter CreateLensSplitter()
        {
            GameObject gameObject = new GameObject("Splitter", typeof(LensSplitter));
            return gameObject.GetComponent<LensSplitter>();
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
        /// Creates a portal that is not linked to another portal.
        /// </summary>
        /// <returns>An unlinked portal</returns>
        public static Portal CreateUnlinkedPortal()
        {
            GameObject gameObject = new GameObject("Portal", typeof(Portal));
            return gameObject.GetComponent<Portal>();
        }
    }
}
