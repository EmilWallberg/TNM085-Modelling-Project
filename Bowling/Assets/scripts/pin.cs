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
            if (linearVelocity[i] >= 0)
                Force[i] -= my * PhysicsEngine.gravity;
            else
                Force[i] += my * PhysicsEngine.gravity;
        }
        //Debug.Log(Force);
        Vector3 acceleration = Force / mass;
        linearVelocity = PhysicsEngine.Euler(linearVelocity, acceleration, Time.fixedDeltaTime);
        transform.position = PhysicsEngine.Euler(transform.position, linearVelocity, Time.fixedDeltaTime);

        Vector3 angularAcceleration = Torq;
        angularVelocity = PhysicsEngine.RungeKutta(angularVelocity, angularAcceleration, Time.fixedDeltaTime);
        apply_rotation(angularVelocity);
    }

}
