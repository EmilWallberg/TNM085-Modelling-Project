using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsEngine {
    public static class Numerical {
        public static Vector3 Euler(Vector3 value, Vector3 f, float h)
        {
            return value + f * h;
        }

        public static GameObject(GameObject current, GameObject[] gameObjects)
        {
            foreach (GameObject go in gameObjects)
            {
                float distance = Vector3.Distance(current.transform.position, go.transform.position);
                if (distance <= radius)
                {

                }
            }
        }
}