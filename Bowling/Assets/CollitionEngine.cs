using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mos.PhysicsEngine;

public class CollitionEngine : MonoBehaviour
{
    float timeStep;
    private void Start()
    {
        timeStep = Time.fixedDeltaTime * PhysicsEngine.timeStep;
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < PhysicsEngine.objectsInScene.Count; i++)
        {
            GameObject obj1 = PhysicsEngine.objectsInScene[i];
            if (obj1.GetComponent<KinematicBody>().velocity.magnitude < 0.001f)
                continue;
            for (int j = i + 1; j < PhysicsEngine.objectsInScene.Count; j++)
            {
                GameObject obj2 = PhysicsEngine.objectsInScene[j];
                if ((obj1.transform.position - obj2.transform.position).magnitude > .4f)
                    continue;
                Vector3 collisionNormal, collisionPoint, linearImpuls, angularImpuls;

                if (PhysicsEngine.GJKCollisionDetection(obj1.GetComponent<MeshCollider>(), obj2.GetComponent<MeshCollider>(), out collisionPoint, out collisionNormal))
                {
                    Debug.DrawLine(collisionPoint, collisionPoint);
                    PhysicsEngine.ImpulsAng(obj1, obj2, collisionNormal, collisionPoint, out linearImpuls, out angularImpuls);
                }
            }
        }
    }
}
