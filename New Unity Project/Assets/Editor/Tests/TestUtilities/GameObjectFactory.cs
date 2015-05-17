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
        /// Creates and returns a LaserTarget game object.
        /// </summary>
        /// <returns>The laser target.</returns>
        public static ILaserReceiver CreateLaserTarget()
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
            GameObject gameObject = new GameObject("Emitter", typeof(LaserEmitter));
            return gameObject.GetComponent<LaserEmitter>();
        }

        /// <summary>
        /// Creates and returns a laser beam.
        /// </summary>
        /// <returns>The laser beam.</returns>
        public static LaserBeam CreateTestBeam()
        {
            return new LaserBeam(Vector3.zero, Vector3.forward, CreateEmitter());
        }
    }
}
