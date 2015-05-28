//----------------------------------------------------------------------------
// <copyright file="MirrorController.cs" company="Delft University of Technology">
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
    using Core.Receiver;
    using UnityEngine;

    /// <summary>
    /// Provides selection and rotation functionality to Mirrors.
    /// </summary>
    public class MirrorController : MonoBehaviour
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
        /// The selected Mirror.
        /// </summary>
        private Mirror selected;

        /// <summary>
        /// Gets or sets the selected Mirror.
        /// </summary>
        public Mirror SelectedMirror
        {
            get
            {
                return this.selected;
            }

            set
            {
                if (this.selected != value)
                {
                    this.ResetHighlight(this.selected);
                    this.selected = value;
                    this.HighlightMirror(this.selected);
                }
            }
        }

        /// <summary>
        /// Initializes this MirrorController and disables state synchronization for this
        /// object.
        /// </summary>
        public void Start()
        {
            NetworkView view = this.GetComponent<NetworkView>();
            view.stateSynchronization = NetworkStateSynchronization.Off;
        }

        /// <summary>
        /// Updates the selected Mirror if the 'M' key is pressed and updates the 
        /// rotation of the selected Mirror.
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                this.UpdateSelectedMirror();
            }

            this.Rotate();
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

                if (hit)
                {
                    if (hitInfo.collider.gameObject.GetComponent<Mirror>() != null)
                    {
                        this.SelectedMirror = hitInfo.collider.gameObject.GetComponent<Mirror>();
                    }
                }
            }
        }

        /// <summary>
        /// Changes the selected Mirror to the next Mirror in sequence.
        /// </summary>
        private void UpdateSelectedMirror()
        {
            Mirror[] mirrors = GameObject.FindObjectsOfType<Mirror>();
            if (mirrors.Length == 0)
            {
                this.SelectedMirror = null;
                return;
            }

            // We need the Mirror at 'index + 2' because:
            //   'index' is the current mirror. 
            //   'index + 1' is the other side of the same mirror.
            //   'index + 2' if the first side of the next mirror.
            int index = Array.IndexOf(mirrors, this.SelectedMirror);
            if (index + 2 >= mirrors.Length)
            {
                this.SelectedMirror = mirrors[0];
            }
            else
            {
                this.SelectedMirror = mirrors[index + 2];
            }
        }

        /// <summary>
        /// Rotates the mirror corresponding with whether the 'A' (left) or 'D' (right) key 
        /// is pressed.
        /// </summary>
        private void Rotate()
        {
            if (this.SelectedMirror != null)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    float t = Time.deltaTime * -90f;
                    this.SelectedMirror.transform.Rotate(0, t, 0);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    float t = Time.deltaTime * 90f;
                    this.SelectedMirror.transform.Rotate(0, t, 0);
                }
            }
        }

        /// <summary>
        /// Highlights the mirror.
        /// </summary>
        /// <param name="mirror">The Mirror to highlight.</param>
        private void HighlightMirror(Mirror mirror)
        {
            if (mirror != null)
            {
                Transform frame = mirror.transform.Find("Frame");
                MeshRenderer mesh = frame.GetComponent<MeshRenderer>();
                mesh.material = this.Highlight;
            }
        }

        /// <summary>
        /// Resets the Highlight.
        /// </summary>
        /// <param name="mirror">The Mirror to reset.</param>
        private void ResetHighlight(Mirror mirror)
        {
            if (mirror != null)
            {
                Transform frame = mirror.transform.Find("Frame");
                MeshRenderer mesh = frame.GetComponent<MeshRenderer>();
                mesh.material = this.Original;
            }
        }
    }
}
