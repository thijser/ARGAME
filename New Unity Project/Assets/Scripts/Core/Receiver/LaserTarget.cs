//----------------------------------------------------------------------------
// <copyright file="LaserTarget.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Receiver
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// A Laser target that loads the next level when Hit with a Laser beam.
    /// </summary>
    public class LaserTarget : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// The name of the opening animation clip.
        /// </summary>
        public const string OpenClipName = "Open";

        /// <summary>
        /// The name of the opening animation state.
        /// </summary>
        public const string OpenedStateName = "Opened";

        /// <summary>
        /// The minimal strength required to make the crystal open.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public float MinimumStrength;

        /// <summary>
        /// The model that defines the crystal.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public MeshRenderer Crystal;

        /// <summary>
        /// The required laser color of this target.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Color TargetColor;

        /// <summary>
        /// The index of the next level.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int NextLevelIndex;

        /// <summary>
        /// Gets or sets a value indicating whether the target is opening.
        /// </summary>
        /// <value><c>true</c> if the target is opening; otherwise, <c>false</c>.</value>
        public bool IsOpening { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Start()
        {
            this.IsOpening = false;
            if (this.NextLevelIndex < 0 || this.NextLevelIndex >= Application.levelCount)
            {
                Debug.LogError("NextLevelIndex is set to " + this.NextLevelIndex +
                    ", but should be between 0 and " + Application.levelCount);
            }
        }

        /// <summary>
        /// Updates the animation properties.
        /// </summary>
        public void LateUpdate()
        {
            GetComponent<Animator>().SetBool("LaserHit", this.IsOpening);
            this.StartCoroutine(this.ResetState());
        }

        /// <summary>
        /// Consumes the Laser beam and loads the next level if the target is fully opened.
        /// </summary>
        /// <param name="sender">The object that sent this event</param>
        /// <param name="args">The arguments that describe the event</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetBool("LaserHit", true);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName(OpenedStateName))
            {
                Application.LoadLevel(this.NextLevelIndex);
            }
        }

        /// <summary>
        /// Ensures the color of the crystal is correct.
        /// </summary>
        public void Update()
        {
            this.EnsureCrystalColor();
        }

        /// <summary>
        /// Ensures the color of the crystal is correct.
        /// </summary>
        private void EnsureCrystalColor()
        {
            this.Crystal.material.SetColor("_Color", this.TargetColor);
        }

        /// <summary>
        /// Resets the shadowed animation state at the end of the current frame.
        /// </summary>
        /// <returns>The IEnumerator instance used for waiting.</returns>
        private IEnumerator ResetState()
        {
            yield return new WaitForEndOfFrame();
            this.IsOpening = false;
        }
    }
}
