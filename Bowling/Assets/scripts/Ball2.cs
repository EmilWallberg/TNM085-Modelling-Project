using Mos.PhysicsEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball2 : KinematicBody
{
    // Start is called before the first frame update
    [SerializeField]
    Vector3 startVelocity = new Vector3(3, 0, 0);

    [SerializeField]
    Vector3 startAngularVelocity = new Vector3(5,0,0);





    [SerializeField]
    float delta = 0.2f;

    Vector3 EulerAngle = Vector3.zero;
    [SerializeField]
    

    bool[] rollingWithoutSlipping = new bool[] { false, false, false };
    float h = 0.01f;


    Vector3 startForce=new Vector3(10,0,0);
    bool rollinWithoutSlipping = false;

    bool angularV_isFaster;
 

    private void Start()
    {
        base.Start();
        inertia = (2 * mass * Mathf.Pow(radius, 2.0f)) / 5; // I = (2mr^2)/5 for sphere
       
        velocity = startVelocity;
        angularVelocity = startAngularVelocity;
        //Force = startForce;

        if(velocity.magnitude > angularVelocity.magnitude) 
        {
            angularV_isFaster = false;
            Debug.Log("v is faster");
        }
        else { angularV_isFaster = true; }
    }
    // Update is called once per frame
    void Update()
    {
        base.FixedUpdate();
        float frictionForce = my * PhysicsEngine.gravity * mass;
        //Friktion
        Vector3 frictionAcceleration = (-my / mass) * velocity;
        Debug.Log(frictionAcceleration);
        Vector3 frictionForce2 = Vector3.zero;
        frictionForce2 = PhysicsEngine.Euler(frictionForce2, frictionAcceleration, h * Time.deltaTime);


        float rollingFriction = (-5 * mass * PhysicsEngine.gravity * delta / radius / 7);
        Vector3 rollingFriction2 = rollingFriction * angularVelocity;

       

        Torq.x = my * PhysicsEngine.gravity*radius;
        Torq.z = my * PhysicsEngine.gravity * radius;
        Vector3 angularAcceleration = Torq / inertia;

       

        if (rollinWithoutSlipping)
        {

            angularVelocity = PhysicsEngine.Euler(angularVelocity, angularAcceleration, h * Time.deltaTime);
            Debug.Log("ROLLING WITHOUT SLIPP");
            angularAcceleration += rollingFriction2;
           
            velocity = angularVelocity * radius;
        }
        else
        {
            Force += frictionForce2;
            Debug.Log("Friction" + frictionForce2);
            angularAcceleration += rollingFriction2;
            angularVelocity = PhysicsEngine.Euler(angularVelocity, angularAcceleration, h * Time.deltaTime);
            Vector3 acceleration = (Force / mass)-(angularAcceleration/mass);
            velocity = PhysicsEngine.Euler(velocity, acceleration, h * Time.deltaTime);

            

            
            if (velocity.magnitude <= angularVelocity.magnitude * radius&& angularV_isFaster){rollinWithoutSlipping = true; Debug.Log("Roll and slipping"); }
            else if (velocity.magnitude >= angularVelocity.magnitude * radius && !angularV_isFaster){rollinWithoutSlipping = true; Debug.Log("Roll and slipping"); }
            
        }


        transform.position = PhysicsEngine.Euler(transform.position, velocity, Time.deltaTime);
        transform.LookAt(transform.position);
       


        EulerAngle = PhysicsEngine.Euler(EulerAngle, angularVelocity, Time.deltaTime);
        transform.eulerAngles = new Vector3(EulerAngle.z, EulerAngle.y, -EulerAngle.x) * Mathf.Rad2Deg;
        

        
    }
}
