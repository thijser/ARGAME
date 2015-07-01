//----------------------------------------------------------------------------
// <copyright file="MirrorController.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Core.Receiver;
    using Network;
    using Projection;
    using UnityEngine;
    using UnityEngine.Assertions;

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
        /// Current rotation speed of mirror.
        /// </summary>
        private float rotationSpeed = 0.0f;

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
                    // Find closest mirror to point
                    Mirror[] mirrors = GameObject.FindObjectsOfType<Mirror>();

                    float closestDistance = float.PositiveInfinity;
                    Mirror closestMirror = null;

                    foreach (Mirror mirror in mirrors)
                    {
                        float d = (hitInfo.point - mirror.transform.position).magnitude;

                        if (d < closestDistance)
                        {
                            closestDistance = d;
                            closestMirror = mirror;
                        }
                    }

                    this.SelectedMirror = closestMirror;
                }
                else
                {
                    this.SelectedMirror = null;
                }
            }
        }

        /// <summary>
        /// Sends a message with the current rotation of the selected mirror.
        /// <para>
        /// The selected mirror should not be null.
        /// </para>
        /// </summary>
        public void SendRotationUpdate()
        {
            Assert.IsNotNull(this.SelectedMirror, "SendRotationUpdate: No Mirror Selected");
            RemoteMarker marker = this.SelectedMirror.GetComponent<RemoteMarker>();
            float rotation = marker.ObjectRotation;

            RotationUpdate update = new RotationUpdate(UpdateType.UpdateRotation, rotation, marker.Id);
            this.SendMessage("OnRotationChanged", update);
            this.SendMessage("OnRotationUpdate", update);
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
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.D))
                {
                    this.rotationSpeed = 0.0f;
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Mouse0))
                {
                    float t = Time.deltaTime * -this.rotationSpeed;
                    this.rotationSpeed = Mathf.Min(90f, this.rotationSpeed + (Time.deltaTime * 45.0f));

                    this.SelectedMirror.GetComponent<RemoteMarker>().ObjectRotation += t;
                    this.SendRotationUpdate();
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Mouse1))
                {
                    float t = Time.deltaTime * this.rotationSpeed;
                    this.rotationSpeed = Mathf.Min(90.0f, this.rotationSpeed + (Time.deltaTime * 45.0f));

                    this.SelectedMirror.GetComponent<RemoteMarker>().ObjectRotation += t;
                    this.SendRotationUpdate();
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
