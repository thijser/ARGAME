﻿//----------------------------------------------------------------------------
// <copyright file="MultiEmitter.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Emitter
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Graphics;
    using UnityEngine;

    /// <summary>
    /// Provides functionality to emit Laser beams with varying properties by
    /// maintaining separate LineRenderers for each Laser type.
    /// </summary>
    public class MultiEmitter : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether the LaserEmitters should be disabled each frame.
        /// <para>
        /// The LaserEmitters can be re-used when needed.
        /// </para>
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public bool DisableEmittersEachFrame = true;

        /// <summary>
        /// Gets an array with the LaserEmitters under this MultiEmitter.
        /// </summary>
        public LaserEmitter[] Emitters
        {
            get
            {
                return this.gameObject.GetComponentsInChildren<LaserEmitter>();
            }
        }

        /// <summary>
        /// Applies the properties from the given Laser to the VolumeLineRenderer.
        /// </summary>
        /// <param name="renderer">The VolumeLineRenderer to configure.</param>
        /// <param name="laser">The Laser beam to use as template.</param>
        /// <param name="prop">The LaserProperties describing the Laser beam.</param>
        /// <returns>The configured LineRenderer.</returns>
        public static VolumeLineRenderer ApplyProperties(VolumeLineRenderer renderer, LaserBeam laser, LaserProperties prop)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            if (laser == null)
            {
                throw new ArgumentNullException("laser");
            }

            if (prop == null)
            {
                throw new ArgumentNullException("prop");
            }

            renderer.UseWorldSpace = true;
            renderer.LineMaterial = laser.Emitter.LineRenderer.LineMaterial;
            renderer.ReceiveShadows = false;
            renderer.CastShadows = false;

            prop.RGBStrengths = laser.Emitter.Properties.RGBStrengths;
            
            return renderer;
        }

        /// <summary>
        /// Disables all Emitters by default if <see cref="MultiEmitter.DisableEmittersEachFrame"/> is true.
        /// </summary>
        public void LateUpdate()
        {
            if (this.DisableEmittersEachFrame)
            {
                this.DisableAll();
            }
        }

        /// <summary>
        /// Removes all attached children.
        /// <para>
        /// This method should be called every frame.
        /// </para>
        /// </summary>
        public void DisableAll()
        {
            foreach (LaserEmitter emitter in this.Emitters)
            {
                emitter.Enabled = false;
            }
        }

        /// <summary>
        /// Deletes all LaserEmitters.
        /// </summary>
        public void DeleteAll()
        {
            foreach (LaserEmitter emitter in this.Emitters)
            {
                GameObject.Destroy(emitter.gameObject);
            }
        }

        /// <summary>
        /// Returns a LaserEmitter for the given Laser beam.
        /// </summary>
        /// <param name="laser">The Laser beam to return a LaserEmitter for.</param>
        /// <returns>The LaserEmitter.</returns>
        public LaserEmitter GetEmitter(LaserBeam laser)
        {
            foreach (LaserEmitter emitter in this.Emitters)
            {
                if (!emitter.Enabled)
                {
                    emitter.Enabled = true;
                    ApplyProperties(emitter.LineRenderer, laser, emitter.Properties);
                    return emitter;
                }
            }

            return this.CreateEmitter(laser);
        }

        /// <summary>
        /// Creates a new LaserEmitter that emits the same type of Laser beam
        /// as the provided one.
        /// <para>
        /// The LaserEmitter is created as a separate game object and
        /// added as a child of this game object.
        /// </para>
        /// <para>
        /// To save resources, it is recommended to call <see cref="MultiEmitter.GetEmitter"/>
        /// instead, which re-uses existing LaserEmitters. However, this method can be called 
        /// to create LaserEmitters beforehand.
        /// </para>
        /// </summary>
        /// <param name="laser">The original Laser beam.</param>
        /// <returns>The created LaserEmitter.</returns>
        public LaserEmitter CreateEmitter(LaserBeam laser)
        {
            GameObject emitterObject = new GameObject("Emitter");
            emitterObject.transform.parent = this.gameObject.transform;
            VolumeLineRenderer renderer = emitterObject.AddComponent<VolumeLineRenderer>();
            LaserEmitter emitter = emitterObject.AddComponent<LaserEmitter>();

            LaserProperties prop = emitterObject.AddComponent<LaserProperties>();

            ApplyProperties(renderer, laser, prop);
            return emitter;
        }
    }
}
