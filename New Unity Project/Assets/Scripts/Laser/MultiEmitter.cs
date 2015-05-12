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
        /// Removes all attached emitters.
        /// </summary>
        public void DisableAll()
        {
            foreach (var emitter in this.gameObject.GetComponentsInChildren<LaserEmitter>())
            {
                GameObject.Destroy(emitter.gameObject);
            }
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
            GameObject emitterObject = new GameObject("Emitter");
            emitterObject.transform.parent = this.gameObject.transform;
            LineRenderer renderer = emitterObject.AddComponent<LineRenderer>();
            LaserEmitter emitter = emitterObject.AddComponent<LaserEmitter>();

            renderer.useWorldSpace = true;
            renderer.materials = laser.Emitter.LineRenderer.materials;
            renderer.receiveShadows = false;
            renderer.SetVertexCount(0);

            return emitter;
        }
    }
}