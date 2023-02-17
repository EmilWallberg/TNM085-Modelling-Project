using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsEngine {
    public static class Numerical {
        public static Vector3 Euler(Vector3 value, Vector3 f, float h)
        {
            return value + f * h;
        }
    }
}