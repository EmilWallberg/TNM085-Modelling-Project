using Mos.PhysicsEngine;
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


    public bool grounded = false;
    public MeshCollider ground;
    private const float groundTime = 0.05f;
    private float GroundTimer = 0.2f;


    private MeshCollider colider;
    protected void Start()
    {
        PhysicsEngine.objectsInScene.Add(gameObject);
        ground = GameObject.Find("ground").GetComponent<MeshCollider>();
        colider = GetComponent<MeshCollider>();
    }
    protected void FixedUpdate()
    {
        Force = Torq = Vector3.zero;
        ground = GameObject.Find("ground").GetComponent<MeshCollider>();
        checkCollision();
        CheckGround();
    }

    private void checkCollision()
    {
        foreach (var obj in PhysicsEngine.objectsInScene)
        {
            if(this != obj)
            {
                Vector3 collitionNormal, collitionPoint, linearImpuls, angularImpuls;

                if (PhysicsEngine.GJKCollisionDetection(colider, obj.GetComponent<MeshCollider>(), out collitionPoint, out collitionNormal))
                {
                    PhysicsEngine.ImpulsAngTest(gameObject, obj, collitionNormal, collitionPoint, out linearImpuls, out angularImpuls);
                    Debug.DrawRay(transform.position + collitionPoint, transform.position + collitionNormal * 100, Color.red, 5f);

                    //Force = linearImpuls / Time.fixedDeltaTime;
                    Torq = -angularImpuls / Time.fixedDeltaTime;

                    linearVelocity += linearImpuls / mass / Time.fixedDeltaTime;
                    Debug.Log("Force: " + Force);
                }
            }
            
        }
    }

    private void CheckGround()
    {
        Vector3 collitionNormal, collitionPoint;
        grounded = PhysicsEngine.GJKCollisionDetection(GetComponent<MeshCollider>(), ground, out collitionPoint, out collitionNormal);
        GroundTimer -= Time.fixedDeltaTime;

        foreach(Vector3 vertex in ground.sharedMesh.vertices)
        {
            Debug.DrawRay(ground.transform.TransformPoint(vertex), Vector3.up);
        }


        if (grounded)
        {
            GroundTimer = groundTime;
            Debug.Log("Ground!");
            linearVelocity.y = 0;
        }else if(GroundTimer < 0)
        {
            Force.y -= PhysicsEngine.gravity;
        }
    }

    protected void apply_rotation(Vector3 EulerAngles)
    {
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.x, Vector3.back);
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.z, Vector3.right);
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.y, Vector3.up);
    }
}
