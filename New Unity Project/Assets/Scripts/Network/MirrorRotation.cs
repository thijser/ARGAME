//----------------------------------------------------------------------------
// <copyright file="MirrorRotation.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Provides selection and rotation functionality to Mirrors.
    /// </summary>
    public class MirrorRotation : MonoBehaviour
    {
        /// <summary>
        /// The Highlighted Material of the Mirror.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Material Highlight;
        
        /// <summary>
        /// The original Material of the Mirror.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Material Original;

        /// <summary>
        /// Indicates whether this mirror is currently selected.
        /// </summary>
        private bool selected = false;

        /// <summary>
        /// Returns the top parent in the hierarchy.
        /// </summary>
        /// <param name="transform">The Transform for which to locate the top parent, not null.</param>
        /// <returns>The top parent.</returns>
        public static Transform GetHighestParent(Transform transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException("trans");
            }

            while (transform.parent != null)
            {
                transform = transform.parent;
            }

            return transform;
        }

        /// <summary>
        /// Updates the location of the cube.
        /// </summary>
        public void Update()
        {
            this.Rotate();
            if (Input.GetMouseButtonDown(0))
            {
                this.ResetHighlight();
                this.selected = false;
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

                if (hit)
                {
                    if (GetHighestParent(hitInfo.transform) == this.transform)
                    {
                        this.selected = true;
                        this.HighlightMirror();
                    }
                }
            }
        }

        /// <summary>
        /// Serializes the mirror to the given BitStream.
        /// </summary>
        /// <param name="stream">The BitStream to serialize to.</param>
        /// <param name="info">The NetworkMessageInfo object describing the connection.</param>
        public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            Quaternion syncPosition = Quaternion.identity;
            if (stream.isWriting)
            {
                syncPosition = transform.rotation;
                stream.Serialize(ref syncPosition);
            }
            else
            {
                stream.Serialize(ref syncPosition);
                transform.rotation = syncPosition;
            }
        }

        /// <summary>
        /// Rotates the mirror corresponding with whether the right or left key is pressed.
        /// </summary>
        private void Rotate()
        {
            if (this.selected)
            {
                if (Input.GetKey("right"))
                {
                    float t = Time.deltaTime * -90f;
                    transform.Rotate(0, t, 0);
                }
                else if (Input.GetKey("left"))
                {
                    float t = Time.deltaTime * 90f;
                    transform.Rotate(0, t, 0);
                }
            }
        }

        /// <summary>
        /// Highlights the mirror.
        /// </summary>
        private void HighlightMirror()
        {
            MeshRenderer mesh = transform.Find("MirrorBase").Find("Cube_002").GetComponent<MeshRenderer>();
            mesh.material = this.Highlight;
        }

        /// <summary>
        /// Resets the Highlight.
        /// </summary>
        private void ResetHighlight()
        {
            MeshRenderer mesh = transform.Find("MirrorBase").Find("Cube_002").GetComponent<MeshRenderer>();
            mesh.material = this.Original;
        }
    }
}
