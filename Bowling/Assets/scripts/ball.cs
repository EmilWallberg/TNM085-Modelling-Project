using System;
using System.Collections.Generic;
using UnityEngine;
using Mos.PhysicsEngine1;
using Unity.VisualScripting;

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
    // Update is called once per frame
    void Update()
    {
        base.Update();
      
        if (pushTime > 0)
        {
            Force += startForce;
            pushTime -= Time.fixedDeltaTime;
        }
        for (int i = 0; i < 3; i+=2)
        {
            if (linearVelocity[i] >= 0)
                Force[i] -= my * PhysicsEngine.gravity;
            else
                Force[i] += my * PhysicsEngine.gravity;

            float rollingFriction = 5 * mass * PhysicsEngine.gravity * delta / radius / 7;
            if (angularVelocity[i] > 0.001)
                Torq[i] -= rollingFriction;
            else if (angularVelocity[i] < -0.001)
                Torq[i] += rollingFriction;
            else angularVelocity[i] = 0;

        if ((linearVelocity[i] - angularVelocity[i]*radius < 0.1f && linearVelocity[i] - angularVelocity[i]*radius > -0.1f) || rollingWithoutSlipping[i])
            {
               
                rollingWithoutSlipping[i] = true;
            }
            else
            {
                if (linearVelocity[i] >= 0)
                    Torq[i] += my * PhysicsEngine.gravity * radius;
                else
                    Torq[i] -= my * PhysicsEngine.gravity * radius;
            }
        }
        Vector3 acceleration = Force / mass;
        linearVelocity = PhysicsEngine.RungeKutta(linearVelocity, acceleration, PhysicsEngine.stepSize);
        Vector3 angularAcceleration = Torq / inertia;
        angularVelocity = PhysicsEngine.RungeKutta(angularVelocity, angularAcceleration, PhysicsEngine.stepSize);

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
                velocity[i] = angularVelocity[i]*radius;
                Debug.Log("Rolling" + i);
            }
            else velocity[i] = linearVelocity[i];
        }

        transform.position = PhysicsEngine.RungeKutta(transform.position, velocity, PhysicsEngine.stepSize);
        EulerAngle = PhysicsEngine.RungeKutta(EulerAngle, angularVelocity, PhysicsEngine.stepSize);
        transform.eulerAngles = new Vector3(EulerAngle.z, EulerAngle.y, -EulerAngle.x) * Mathf.Rad2Deg;
    }
}
