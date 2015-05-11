//----------------------------------------------------------------------------
// <copyright file="MultiEmitter.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;
    
    /// <summary>
    /// Provides functionality to emit Laser beams with varying properties by
    /// maintaining separate LineRenderers for each Laser type.
    /// </summary>
    public class MultiEmitter : MonoBehaviour
    {
        /// <summary>
        /// A List with the LaserEmitters in this MultiEmitter.
        /// </summary>
        private List<LaserEmitter> emitters;

        /// <summary>
        /// Gets a read-only version of the LaserEmitters in this MultiEmitter.
        /// </summary>
        public ReadOnlyCollection<LaserEmitter> Emitters
        {
            get
            {
                return this.emitters.AsReadOnly();
            }
        }

        /// <summary>
        /// Returns a LaserEmitter that produces a Laser beam with the same 
        /// properties as the provided Laser beam.
        /// </summary>
        /// <param name="laser">The Laser beam.</param>
        /// <returns>A LaserEmitter for the same Laser type.</returns>
        public LaserEmitter GetEmitter(Laser laser)
        {
            LineRenderer renderer = laser.Emitter.LineRenderer;
            LaserEmitter emitter = this.emitters.Find(e => e.LineRenderer == renderer);
            
            if (emitter == null)
            {
                emitter = this.CreateEmitter(laser);
            }

            return emitter;
        }

        /// <summary>
        /// Creates a new LaserEmitter that emits the same type of Laser beam 
        /// as the provided one.
        /// <para>
        /// The LaserEmitter is created as a separate game object and 
        /// added as a child of this game object.
        /// </para>
        /// </summary>
        /// <param name="laser">The original Laser beam.</param>
        /// <returns>The created LaserEmitter.</returns>
        public LaserEmitter CreateEmitter(Laser laser)
        {
            GameObject emitterObject = new GameObject("Emitter_" + this.emitters.Count);
            emitterObject.transform.parent = this.gameObject.transform;
            LineRenderer renderer = emitterObject.AddComponentAsCopy(laser.Emitter.LineRenderer);
            LaserEmitter emitter = emitterObject.AddComponent<LaserEmitter>();

            // Reset the vertex count to prevent the newly created LineRenderer from drawing the 
            // same path as the original.
            renderer.SetVertexCount(0);
            this.emitters.Add(emitter);
            return emitter;
        }
    }
}