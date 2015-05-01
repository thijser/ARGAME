using System;
using UnityEngine;

namespace Laser {
    public class HitEventArgs : EventArgs {
        public Laser laser { get; set; }
        public Vector3 normal { get; set; }
    }
}
