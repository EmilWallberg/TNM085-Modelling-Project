using Mos.PhysicsEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicBody : MonoBehaviour
{
    public float mass = 1;
    public float radius = 1;
    public float inertia = 1;
    public Vector3 linearVelocity = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;

    private Vector3 Force = Vector3.zero, torq = Vector3.zero;
    public Vector3 velocity = Vector3.zero;


    public void Start()
    {
        PhysicsEngine.objectsInScene.Add(gameObject);
    }
    public void Update()
    {
      checkCollision(); 
    }

    public void checkCollision()
    {
        foreach (var obj in PhysicsEngine.objectsInScene)
        {
            if(this != obj)
            {
                Vector3 collitionNormal, collitionPoint, linearImpuls, angularImpuls;

                if (PhysicsEngine.GJKCollisionDetection(GetComponent<MeshFilter>(), obj.GetComponent<MeshFilter>(), out collitionPoint, out collitionNormal))
                {
                    PhysicsEngine.ImpulsAng(gameObject, obj, collitionNormal, collitionPoint, out linearImpuls, out angularImpuls);
                    Debug.Log("A fine hit");
                    Debug.Log(collitionPoint);
                    Debug.Log(collitionNormal);
                    Debug.DrawRay(transform.position + collitionPoint, transform.position + collitionNormal * 100, Color.red, 5f);

                    Force = linearImpuls / mass;
                    torq = angularImpuls / inertia;
                }
            }
            
        }
   
    }
}
