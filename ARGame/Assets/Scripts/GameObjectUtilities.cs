using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GameObjectUtilities
    {
        public static void SetEnabled(this GameObject obj, bool enabled)
        {
            // Disable rendering of this component
            obj.GetComponent<MeshRenderer>().enabled = enabled;

            // And of its children
            var subComponents = obj.GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in subComponents)
            {
                renderer.enabled = enabled;
            }
        }
    }
}
