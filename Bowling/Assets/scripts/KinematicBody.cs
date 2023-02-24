using Mos.PhysicsEngine1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicBody : MonoBehaviour
{
    public float mass = 1;
    public float radius = 1;
    public float inertia = 1;
    public float my = .4f;
    public Vector3 linearVelocity = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;

    protected Vector3 Force = Vector3.zero, Torq = Vector3.zero;
    public Vector3 velocity = Vector3.zero;


    public void Start()
    {
        PhysicsEngine.objectsInScene.Add(gameObject);
    }
    public void Update()
    {
        Force = Torq = Vector3.zero;
        checkCollision(); 
    }

    public void checkCollision()
    {
        foreach (var obj in PhysicsEngine.objectsInScene)
        {
            if(this != obj)
            {
                Vector3 collitionNormal, collitionPoint, linearImpuls, angularImpuls;

                if (PhysicsEngine.GJKCollisionDetection(GetComponent<MeshCollider>(), obj.GetComponent<MeshCollider>(), out collitionPoint, out collitionNormal))
                {
                    PhysicsEngine.ImpulsAngTest(gameObject, obj, collitionNormal, collitionPoint, out linearImpuls, out angularImpuls);
                    Debug.DrawRay(transform.position + collitionPoint, transform.position + collitionNormal * 100, Color.red, 5f);

                    Force = linearImpuls / PhysicsEngine.stepSize;
                    Torq = -angularImpuls / PhysicsEngine.stepSize;

                    //velocity += linearImpuls / mass / Time.fixedDeltaTime;
                    Debug.Log("Force: " + Force);
                }
            }
            
        }
   
    }
}
