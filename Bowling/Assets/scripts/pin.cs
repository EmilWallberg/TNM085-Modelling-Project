using Mos.PhysicsEngine;
using UnityEngine;

public class pin : KinematicBody
{
    // Start is called before the first frame update
    const float linearVelocityThreshold = 0.0001f;

    float frictionCoefficient;
    private void Start()
    {
        base.Start();
        inertia = (2 * mass * Mathf.Pow(radius, 2.0f)) / 5; // I = (2mr^2)/5 for sphere
        frictionCoefficient = my * PhysicsEngine.gravity * mass;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();

        for (int i = 0; i < 3; i += 2)
        {
            if (linearVelocity[i] > linearVelocityThreshold)
                Force[i] -= frictionCoefficient;
            else if (linearVelocity[i] < -linearVelocityThreshold)
                Force[i] += frictionCoefficient;
            else linearVelocity[i] = 0;
        }
        //Debug.Log(Force);
        Vector3 acceleration = Force / mass;
        linearVelocity = PhysicsEngine.Euler(linearVelocity, acceleration, timeStep);
        velocity = linearVelocity;
        transform.position = PhysicsEngine.Euler(transform.position, velocity, timeStep);

        //Vector3 angularAcceleration = Torq;
        //angularVelocity = PhysicsEngine.Euler(angularVelocity, angularAcceleration, timeStep);
        apply_rotation(angularVelocity * timeStep * Mathf.Rad2Deg);
    }

}
