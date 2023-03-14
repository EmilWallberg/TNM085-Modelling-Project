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
            if (obj1.GetComponent<KinematicBody>().velocity.magnitude < 0.02f)
                continue;
            for (int j = i + 1; j < PhysicsEngine.objectsInScene.Count; j++)
            {
                GameObject obj2 = PhysicsEngine.objectsInScene[j];
                if ((obj1.transform.position - obj2.transform.position).magnitude > .5f)
                    continue;
                //if (Vector3.Dot(obj1.GetComponent<KinematicBody>().velocity, obj2.GetComponent<KinematicBody>().velocity) < 0.0001 ||(
                //    obj1.GetComponent<KinematicBody>().velocity.magnitude != 0 &&
                //    obj1.GetComponent<KinematicBody>().velocity.magnitude != 0))
                //    continue;
                Vector3 collisionNormal, collisionPoint;

                if (PhysicsEngine.GJKCollisionDetection(obj1.GetComponent<MeshCollider>(), obj2.GetComponent<MeshCollider>(), out collisionPoint, out collisionNormal))
                {
                    Color c = obj1.name == "BowlingBall" ? Color.green : Color.magenta;
                    Debug.DrawLine(obj1.transform.TransformPoint(collisionPoint), obj1.transform.TransformPoint(collisionPoint +collisionNormal), c);
                    PhysicsEngine.ImpulsAng(obj1, obj2, collisionNormal, collisionPoint);

                    ball b;
                    obj1.TryGetComponent<ball>(out b);
                    if(b != null)
                    {
                        b.rollingWithoutSlipping[0] = b.rollingWithoutSlipping[1] = b.rollingWithoutSlipping[2] = false;
                    }
                    obj2.TryGetComponent<ball>(out b);
                    if (b != null)
                    {
                        b.rollingWithoutSlipping[0] = b.rollingWithoutSlipping[1] = b.rollingWithoutSlipping[2] = false;
                    }
                }
            }
        }
    }
}
