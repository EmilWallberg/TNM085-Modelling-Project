using Mos.PhysicsEngine;
using UnityEngine;

public class pin : KinematicBody
{
    // Start is called before the first frame update


    Vector3 EulerAngle = Vector3.zero;
    void Start()
    {
        base.Start();
        inertia = mass * Mathf.Pow(radius, 2.0f)/2;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();

        for (int i = 0; i < 3; i += 2)
        {
            if (velocity[i] > 0.001f)
                Force[i] -= my * PhysicsEngine.gravity * mass;
            else if (velocity[i] < 0.001f)
                Force[i] += my * PhysicsEngine.gravity * mass;
            else velocity[i] = 0;
        }
        //Debug.Log(Force);
        Vector3 acceleration = Force / mass;
        velocity = PhysicsEngine.RungeKutta(velocity, acceleration, timeStep);
        transform.position = PhysicsEngine.RungeKutta(transform.position, velocity, timeStep);

        Vector3 angularAcceleration = Torq;
        //angularVelocity = PhysicsEngine.RungeKutta(angularVelocity, angularAcceleration, timeStep);
        apply_rotation(angularVelocity * timeStep * Mathf.Rad2Deg);
    }

}
