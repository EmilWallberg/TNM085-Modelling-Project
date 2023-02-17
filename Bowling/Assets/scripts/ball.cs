using System;
using System.Collections.Generic;
using UnityEngine;
using PhysicsEngine;

[Serializable]
public class ball : KinematicBody
{

    [SerializeField]
    Vector3 startForce = new Vector3(200, 0, 50);
    [SerializeField]
    float pushTime = 4f;

    [SerializeField]
    float radius = 2;
    float delta = 0.0000000002f;

    Vector3 EulerAngle = Vector3.zero;

    float my = 0.5f;

    float I;
    bool[] rollingWithoutSlipping = new bool[] { false, false, false };

    float h = 0.2f;

    private void Start()
    {
        I = (2 * mass * Mathf.Pow(radius, 2.0f)) / 5; // I = (2mr^2)/5 for sphere
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 Force = Vector3.zero, torq = Vector3.zero;
        if (pushTime > 0)
        {
            Force += startForce;
            pushTime -= Time.deltaTime;
        }
        for (int i = 0; i < 3; i+=2)
        {
            if (linearVelocity[i] >= 0)
                Force[i] -= my * PhysicalAttributes.gravity;
            else
                Force[i] += my * PhysicalAttributes.gravity;

            float rollingFriction = 5 * mass * PhysicalAttributes.gravity * delta / radius / 7;
            if (angularVelocity[i] > 0)
                torq[i] -= rollingFriction;
            else
                torq[i] += rollingFriction;

        if ((linearVelocity[i] - angularVelocity[i]*radius < 0.1f && linearVelocity[i] - angularVelocity[i]*radius > -0.1f) || rollingWithoutSlipping[i])
            {
                Debug.Log(i);
                rollingWithoutSlipping[i] = true;
            }
            else
            {
                if (linearVelocity[i] >= 0)
                    torq[i] += my * PhysicalAttributes.gravity * radius;
                else
                    torq[i] -= my * PhysicalAttributes.gravity * radius;
            }
        }
        Vector3 acceleration = 1 / mass * Force;
        linearVelocity = Numerical.Euler(linearVelocity, acceleration, h * Time.deltaTime);
        Vector3 angularAcceleration = torq / I;
        angularVelocity = Numerical.Euler(angularVelocity, angularAcceleration, h * Time.deltaTime);

        for (int i = 0; i < 3; i++)
        {
            if (rollingWithoutSlipping[i])
            {
                velocity[i] = angularVelocity[i];
            }
            else velocity[i] = linearVelocity[i];
        }

        transform.position = Numerical.Euler(transform.position, velocity, Time.deltaTime);
        EulerAngle = Numerical.Euler(EulerAngle, angularVelocity, Time.deltaTime);
        transform.rotation = Quaternion.Euler(EulerAngle);
    }
}
