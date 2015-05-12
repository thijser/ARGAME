//----------------------------------------------------------------------------
// <copyright file="BeamSplitter.cs" company="Delft University of Technology">
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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// An object that splits the beam into a reflection and pass-through ray.
    /// </summary>
    public class BeamSplitter : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// Emitter object that is cloned for each incoming Laser to produce the
        /// pass-through ray.
        /// </summary>
        private GameObject protoEmitter;

        /// <summary>
        /// Emitter objects that are currently being used for pass-through rays.
        /// </summary>
        private List<GameObject> emitterPool = new List<GameObject>();

        /// <summary>
        /// Index of emitter object in pool that will be used for the next
        /// incoming ray (OnLaserHit call).
        /// </summary>
        private int nextEmitterIndex = 0;

        /// <summary>
        /// Initialize by finding reference emitter object.
        /// </summary>
        public void Start()
        {
            this.protoEmitter = transform.Find("ProtoEmitter").gameObject;
        }

        /// <summary>
        /// Get an emitter object to produce the pass-through ray for the
        /// current laser hit.
        /// </summary>
        /// <returns>Unique emitter object</returns>
        public GameObject GetEmitter()
        {
            if (this.nextEmitterIndex >= this.emitterPool.Count)
            {
                var newEmitter = Instantiate<GameObject>(this.protoEmitter);
                newEmitter.SetActive(true);
                this.emitterPool.Add(newEmitter);
            }

            return this.emitterPool[this.nextEmitterIndex++];
        }

        /// <summary>
        /// Reflects the argument Laser beam and emits a new Laser beam that
        /// passes through.
        /// </summary>
        /// <param name="sender">The sender of the event, ignored here.</param>
        /// <param name="args">The EventArgs object that describes the event.</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            // Create a new ray coming out of the other side with the same direction
            // as the original ray. Forward needs to be negative, see LaserEmitter.
            var passThroughEmitter = this.GetEmitter();

            passThroughEmitter.transform.position = args.Point + (args.Laser.Direction * 0.1f);
            passThroughEmitter.transform.forward = -args.Laser.Direction;
            LaserProperties propertiesPre = args.Laser.Emitter.GetComponent<LaserProperties>();
            LaserProperties propertiesPost = passThroughEmitter.GetComponent<LaserProperties>();
            propertiesPost.RGBStrengths = propertiesPre.RGBStrengths / 2;

            // Create the second ray, reflecting off surface like a mirror
            Mirror.CreateReflection(args.Laser, args.Normal);
        }

        /// <summary>
        /// Remove any emitters from the pool that aren't needed anymore, generally
        /// because less lasers hit the beam splitter since this frame. Because the
        /// LaserEmitter objects use Update(), LateUpdate() is guaranteed to be called
        /// after all lasers have been created.
        /// </summary>
        public void LateUpdate()
        {
            if (this.nextEmitterIndex < this.emitterPool.Count)
            {
                for (int i = this.nextEmitterIndex; i < this.emitterPool.Count; i++)
                {
                    GameObject.Destroy(this.emitterPool[i]);
                }

                this.emitterPool.RemoveRange(this.nextEmitterIndex, this.emitterPool.Count - this.nextEmitterIndex);
            }

            // Start at the first emitter for the first hit again next frame
            this.nextEmitterIndex = 0;
        }
    }
}
