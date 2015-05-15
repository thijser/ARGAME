using UnityEngine;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Meta.Apps.MetaSDKGuide
{

    /// <summary>
    /// Controls the HUDCube in the Meta SDK Guide scene
    /// </summary>
    public class HudCube : MonoBehaviour
    {

        [SerializeField]
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public GameObject _hudCube;


        /// <summary>
        /// Sets the cube's position to the centre of the hud so that it is not cropped off
        /// </summary>
        public void SetHudCubePos()
        {
            if (MetaCore.Instance.transform.rotation != Quaternion.identity)
            {
                _hudCube.transform.position = Camera.main.transform.position;
                _hudCube.transform.Translate(Camera.main.transform.forward * 0.4f, relativeTo: Space.World);
            }
        }
    }

}
