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

    public GameObject wall1, wall2;
    public MeshCollider wM1, wM2;
    public bool hit_wall1;
    public bool hit_wall2;
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
        //checkCollision();
        //checkWallCollision();
        tempGroundcheck();
        //tempWallcheck();
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
                    //float energy = (mass * Mathf.Pow(velocity.magnitude, 2f) / 2f + obj.GetComponent<KinematicBody>().mass * Mathf.Pow(obj.GetComponent<KinematicBody>().velocity.magnitude, 2f) / 2f);
                    //Debug.Log("Before: " + energy);

                    PhysicsEngine.ImpulsAng(gameObject, obj, collisionNormal, collisionPoint);
                    Debug.DrawRay(transform.position - collisionPoint, transform.position - collisionPoint + collisionNormal, Color.red, timeStep * 8);

                    //Force = linearImpuls / timeStep;
                    //Torq =  angularImpuls / timeStep;
                    //Debug.Log(name + " | " + linearImpuls / mass);
                    //linearVelocity += linearImpuls / mass;
                    //Debug.Log("Force: " + Force);
                    //PhysicsEngine.ImpulsAngTest(gameObject, obj, collisionNormal, collisionPoint, out linearImpuls, out angularImpuls);
   

                    //Force = linearImpuls / Time.fixedDeltaTime;
                    /*
                   Vector3 newV1 = v1 + (J * vColissionNormal) / m1;
                   Vector3 newW1 = w1 + (Vector3.Cross(vColissionPoint1, J * vColissionNormal)) / m1;
                   Vector3 newV2 = v2 - (J * vColissionNormal) / m2; ;
                   Vector3 newW2 = w2 - (Vector3.Cross(vColissionPoint2, J * vColissionNormal)) / m2;

                   Debug.Log("v2 = " + v2);

                   listOfVel.Add(newV1); listOfVel.Add(newW1); listOfVel.Add(newV2); listOfVel.Add(newW2);
                   */
                    //angularVelocity += Vector3.Cross(collisionPoint - transform.position, linearImpuls);
                    //energy = (mass * Mathf.Pow(velocity.magnitude, 2f) / 2f + obj.GetComponent<KinematicBody>().mass * Mathf.Pow(obj.GetComponent<KinematicBody>().velocity.magnitude, 2f) / 2f);
                    //Debug.Log("After: " + energy);
                }
            }
            
        }
    }

    private void CheckGround()
    {

        Vector3 collitionNormal, collitionPoint;
        grounded = PhysicsEngine.GJKCollisionDetection(GetComponent<MeshCollider>(), ground, out collitionPoint, out collitionNormal);
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
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.x, Vector3.forward);
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.z, Vector3.right);
        transform.rotation *= Quaternion.AngleAxis(EulerAngles.y, Vector3.up);
    }

    private void checkWallCollision()
    {

        Vector3 collisionNormal, collisionPoint;
        wM1 = wall1.GetComponent<MeshCollider>();
        wM2 = wall2.GetComponent<MeshCollider>();
        hit_wall1 = PhysicsEngine.GJKCollisionDetection(colider, wM1, out collisionNormal, out collisionPoint);
        hit_wall2 = PhysicsEngine.GJKCollisionDetection(colider, wM2, out collisionNormal, out collisionPoint);

        if (hit_wall1)
        {
            Debug.Log(this.name + " hitwall1");
        }
        else if(hit_wall2) { Debug.Log(this.name + " hitwall1"); }
    }
    private void tempWallcheck()
    {
        float boundry = 0.5f-this.radius;
        
        if (transform.position.z < -boundry||transform.position.z>boundry)
        {
            float energyLoss = 0.95f;
            float angle = Mathf.Atan2(linearVelocity.x, linearVelocity.z) * Mathf.Rad2Deg;

            Vector3 axis = Vector3.Cross(transform.position, Vector3.right);

            Vector3 rot = Quaternion.AngleAxis(angle, axis) * transform.position;
            linearVelocity = rot * energyLoss;
       

        }

    }
    private void tempGroundcheck()
    {
        if (this.name == "BowlingBall")
        {
            if (transform.position.y < 0.18)
            {
                Vector3 resetY = transform.position;
                resetY.y = 0.18f;
                transform.position = resetY;
            }else if(transform.position.y > 0.18) { Force.y -= PhysicsEngine.gravity; }
        }
        else
        {
            if (transform.position.y < 0.24)
            {
                Vector3 resetY = transform.position;
                resetY.y = 0.24f;
                transform.position = resetY;
            }
            else if (transform.position.y > 0.24) { Force.y -= PhysicsEngine.gravity; }
        }
    }
}
