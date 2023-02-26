using System;
using UnityEngine;
using Mos.PhysicsEngine;

[Serializable]
public class ball : KinematicBody
{

    [SerializeField]
    Vector3 startForce = new Vector3(200, 0, 100);
    [SerializeField]
    float pushTime = 4f;

    [SerializeField]
    float delta = 0.0002f;

    float widthPlayfield = 1f;

    Vector3 EulerAngle = Vector3.zero;

    bool[] rollingWithoutSlipping = new bool[] { false, false, false };

    bool hasHit = false;
    private void Start()
    {
        base.Start();
        inertia = (2 * mass * Mathf.Pow(radius, 2.0f)) / 5; // I = (2mr^2)/5 for sphere
    }



    const float RollingFrictionCoefficient = 5f / 7f;
    const float AngularVelocityThreshold = 0.001f;
    const float RollingWithoutSlippingThreshold = 0.1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        if (pushTime > 0)
        {
            Force += startForce;
            pushTime -= Time.fixedDeltaTime;
        }

        float frictionCoefficient = my * PhysicsEngine.gravity;
        float rollingFrictionCoefficient = RollingFrictionCoefficient * mass * PhysicsEngine.gravity * delta / radius;

        for (int i = 0; i < 3; i += 2)
        {
            if (linearVelocity[i] >= 0)
                Force[i] -= frictionCoefficient;
            else
                Force[i] += frictionCoefficient;

            if (angularVelocity[i] > AngularVelocityThreshold)
                Torq[i] -= rollingFrictionCoefficient;
            else if (angularVelocity[i] < -AngularVelocityThreshold)
                Torq[i] += rollingFrictionCoefficient;
            else angularVelocity[i] = 0;

            if (!rollingWithoutSlipping[i])
            {
                if (Mathf.Abs(linearVelocity[i]-angularVelocity[i]*radius) < RollingWithoutSlippingThreshold)
                {
                    rollingWithoutSlipping[i] = true;
                }
                else
                {
                    float torque = frictionCoefficient * radius;
                    if (linearVelocity[i] < 0)
                        torque *= -1;

                    Torq[i] += torque;
                }
            }
        }

        Vector3 acceleration = Force / mass;
        linearVelocity = PhysicsEngine.RungeKutta(linearVelocity, acceleration, Time.fixedDeltaTime);

        Vector3 angularAcceleration = Torq / inertia;
        angularVelocity = PhysicsEngine.RungeKutta(angularVelocity, angularAcceleration, Time.fixedDeltaTime);

        
        if (Mathf.Abs(transform.position.z) > widthPlayfield / 2)
        {
            Debug.Log("Wall");
            if (velocity.z > 0) {
                Debug.Log(1);
                linearVelocity.z = -velocity.z;
            }
            else {
                Debug.Log(2);
                linearVelocity.z = velocity.z;
            }
        }
        
 


        for (int i = 0; i < 3; i++)
        {
            if (rollingWithoutSlipping[i])
            {
                velocity[i] = angularVelocity[i] * radius;
            }
            else
            {
                velocity[i] = linearVelocity[i];
            }
        }

        transform.position = PhysicsEngine.RungeKutta(transform.position, velocity, Time.fixedDeltaTime);
        apply_rotation(angularVelocity);
    }

}
