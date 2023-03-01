using Mos.PhysicsEngine;
using UnityEngine;

public class KinematicBody : MonoBehaviour
{
    public float simulationTime = 0;
    public float mass = 1;
    public float radius = 1;
    public float inertia = 1;
    public float my = .4f;
    public Vector3 linearVelocity = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;

    protected Vector3 Force = Vector3.zero, Torq = Vector3.zero;
    public Vector3 velocity = Vector3.zero;


    public bool grounded = false;
    public MeshCollider ground;
    private const float groundTime = 0.05f;
    private float GroundTimer = 0.05f;

    protected float timeStep;


    private MeshCollider colider;
    protected void Start()
    {
        timeStep = Time.fixedDeltaTime * PhysicsEngine.timeStep;
        PhysicsEngine.objectsInScene.Add(gameObject);
        ground = GameObject.Find("ground").GetComponent<MeshCollider>();
        colider = GetComponent<MeshCollider>();
    }
    protected void FixedUpdate()
    {
        simulationTime += timeStep;
        Force = Torq = Vector3.zero;
        checkCollision();
        //CheckGround();
    }

    private void checkCollision()
    {
        foreach (var obj in PhysicsEngine.objectsInScene)
        {
            if(this != obj)
            {
                Vector3 collisionNormal, collisionPoint, linearImpuls, angularImpuls;

                if (PhysicsEngine.GJKCollisionDetection(colider, obj.GetComponent<MeshCollider>(), out collisionPoint, out collisionNormal))
                {
                    PhysicsEngine.ImpulsAngTest(gameObject, obj, collisionNormal, collisionPoint, out linearImpuls, out angularImpuls);
                    Debug.DrawRay(transform.position + collisionPoint, transform.position + collisionNormal, Color.red, timeStep * 8);

                    Force = linearImpuls / timeStep;
                    Torq = -angularImpuls;
                    Debug.Log(name + " | " + linearImpuls / mass);
                    //linearVelocity += linearImpuls / mass;
                    //Debug.Log("Force: " + Force);
                }
            }
            
        }
    }

    private void CheckGround()
    {
        Vector3 collisionNormal, collisionPoint;
        grounded = PhysicsEngine.GJKCollisionDetection(GetComponent<MeshCollider>(), ground, out collisionPoint, out collisionNormal);
        GroundTimer -= Time.fixedDeltaTime;

        foreach(Vector3 vertex in ground.sharedMesh.vertices)
        {
            Debug.DrawRay(ground.transform.TransformPoint(vertex), Vector3.up/4);
        }

        if (grounded)
        {
            GroundTimer = groundTime;
            Debug.Log(this.name + " Ground!");
            linearVelocity.y = 0;
        }else if(GroundTimer < 0)
        {
            Force.y -= PhysicsEngine.gravity * mass;
        }
    }

    protected void apply_rotation(Vector3 EulerAngles)
    {
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.x, Vector3.back);
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.z, Vector3.right);
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.y, Vector3.up);
    }
}
