using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Laser {
    ///<summary>
    ///Emitter of Laser beams.
    ///</summary>
    public class LaserEmitter : MonoBehaviour {
        ///<summary>
        ///The LineRenderer used for drawing the Laser beams.
        ///</summary>
        public LineRenderer lineRenderer;
        private List<Laser> segments = new List<Laser>();

        ///<summary>
        ///Recomputes the path of the Lasers and redraws the scene.
        ///</summary>
        public void Update() {
            Clear();
            MakeLaser();
            Render();
        }

        ///<summary>
        ///Creates the Laser beam emitted from this LaserEmitter.
        ///</summary>
        public void MakeLaser() {
            Vector3 pos = gameObject.transform.position;
            Vector3 dir = -gameObject.transform.forward;
            Laser l = new Laser(pos, dir, this);
            l.Create();
        }

        ///<summary>
        ///Clears the current Laser beam.
        ///</summary>
        public void Clear() {
            segments.Clear();
        }

        ///<summary>
        ///Renders the Laser beam using the LineRenderer.
        ///</summary>
        public void Render() {
            lineRenderer.SetVertexCount(segments.Count + 1);
            Vector3 renderOrigin = segments[0].origin;
            lineRenderer.SetPosition(0, renderOrigin);
            for (int i = 0; i < segments.Count; i++) {
                lineRenderer.SetPosition(i + 1, segments[i].endpoint);
            }
        }

        ///<summary>
        ///Adds a Laser segment to this LaserEmitter.
        ///This causes the segment to be drawn using the LineRenderer in this
        ///LaserEmitter.
        ///</summary>
        public void AddLaser(Laser laser) {
            segments.Add(laser);
        }
    }
}
