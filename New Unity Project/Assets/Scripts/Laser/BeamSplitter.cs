//----------------------------------------------------------------------------
// <copyright file="Mirror.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// An object that splits the beam into a reflection and pass-through ray.
    /// </summary>
    public class BeamSplitter : MonoBehaviour, ILaserReceiver {
        private GameObject protoEmitter;

        private GameObject passThroughEmitter;

        public void Start() {
            protoEmitter = transform.Find("ProtoEmitter").gameObject;
        }

        /// <summary>
        /// Reflects the argument Laser beam and emits a new Laser beam that
        /// passes through.
        /// </summary>
        /// <param name="sender">The sender of the event, ignored here.</param>
        /// <param name="args">The EventArgs object that describes the event.</param>
        public void OnLaserHit(object sender, HitEventArgs args) {
            if (args == null) {
                throw new ArgumentNullException("args");
            }

            if (passThroughEmitter == null) {
                passThroughEmitter = Instantiate<GameObject>(protoEmitter);
            }

            passThroughEmitter.SetActive(true);

            passThroughEmitter.transform.position = args.Point + args.Laser.Direction * 0.1f;
            passThroughEmitter.transform.forward = -args.Laser.Direction;

            Mirror.CreateReflection(args.Laser, args.Normal);
        }
    }
}
