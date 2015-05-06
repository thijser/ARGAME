﻿//----------------------------------------------------------------------------
// <copyright file="LaserEmitter.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser 
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;

    /// <summary>
    /// Emitter of Laser beams.
    /// </summary>
    public class LaserEmitter : MonoBehaviour 
    {
        /// <summary>
        /// The LineRenderer used for drawing the Laser beams.
        /// </summary>
        public LineRenderer LineRenderer;

        /// <summary>
        /// A List of segments that make up the whole Laser beam this LaserEmitter emits.
        /// </summary>
        private List<Laser> segments = new List<Laser>();
        public ReadOnlyCollection<Laser> Segments
        {
            get
            {
                return segments.AsReadOnly();
            }
        }

        /// <summary>
        /// Updates the path of the Lasers and redraws the scene.
        /// </summary>
        public void Update() 
        {
            this.Clear();
            this.MakeLaser();
            this.Render();
        }

        /// <summary>
        /// Creates the Laser beam emitted from this LaserEmitter.
        /// </summary>
        public void MakeLaser() 
        {
            Vector3 pos = gameObject.transform.position;
            Vector3 dir = -gameObject.transform.forward;
            Laser l = new Laser(pos, dir, this);
            l.Create();
        }

        /// <summary>
        /// Clears the current Laser beam.
        /// </summary>
        public void Clear()
        {
            this.segments.Clear();
        }

        /// <summary>
        /// Renders the Laser beam using the LineRenderer.
        /// </summary>
        public void Render() 
        {
            this.LineRenderer.SetVertexCount(this.segments.Count + 1);
            Vector3 renderOrigin = this.segments[0].Origin;
            this.LineRenderer.SetPosition(0, renderOrigin);
            for (int i = 0; i < this.segments.Count; i++) 
            {
                this.LineRenderer.SetPosition(i + 1, this.segments[i].Endpoint);
            }
        }

        /// <summary>
        /// Adds a Laser segment to this LaserEmitter.
        /// This causes the segment to be drawn using the LineRenderer in this
        /// LaserEmitter.
        /// </summary>
        /// <param name="laser">The Laser beam to add.</param>
        public void AddLaser(Laser laser) 
        {
            this.segments.Add(laser);
        }
    }
}
