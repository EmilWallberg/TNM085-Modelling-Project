using System;
using UnityEngine;
using Mos.PhysicsEngine;

[Serializable]
public class ball : KinematicBody
{
    [SerializeField]
    float pushTime = 4f;

    [SerializeField]
    float delta = 0.0002f;

    public bool[] rollingWithoutSlipping = new bool[] { false, false, false };

    const float RollingFrictionCoefficient = 5f / 7f;
    const float AngularVelocityThreshold = 0.0001f;
    const float linearVelocityThreshold = 0.0001f;
    const float RollingWithoutSlippingThreshold = 0.01f;

    float frictionCoefficient;
    float rollingFrictionCoefficient;
    private void Start()
    {
        base.Start();
        inertia = (2 * mass * Mathf.Pow(radius, 2.0f)) / 5; // I = (2mr^2)/5 for sphere
        frictionCoefficient = my * PhysicsEngine.gravity * mass;
        rollingFrictionCoefficient = RollingFrictionCoefficient * mass * PhysicsEngine.gravity * delta / radius;
    }

    // Update is called once per frame
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

            if (angularVelocity[i] > AngularVelocityThreshold)
                Torq[i] -= rollingFrictionCoefficient;
            else if (angularVelocity[i] < -AngularVelocityThreshold)
                Torq[i] += rollingFrictionCoefficient;
            else angularVelocity[i] = 0;

            if (!rollingWithoutSlipping[i])
            {
                if (Mathf.Abs(linearVelocity[i]-angularVelocity[i]*radius) < RollingWithoutSlippingThreshold || rollingWithoutSlipping[i])
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
        linearVelocity = PhysicsEngine.Euler(linearVelocity, acceleration, timeStep);

        Vector3 angularAcceleration = Torq / inertia;
        angularVelocity = PhysicsEngine.Euler(angularVelocity, angularAcceleration, timeStep);

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

        transform.position = PhysicsEngine.Euler(transform.position, velocity, timeStep);
        apply_rotation(new Vector3(-angularVelocity.x, angularVelocity.y, angularVelocity.z) * timeStep * Mathf.Rad2Deg);
    }

    protected override void HandleImpact()
    {
        base.HandleImpact();
        rollingWithoutSlipping[0] = rollingWithoutSlipping[1] = rollingWithoutSlipping[2] = false;
    }

}
