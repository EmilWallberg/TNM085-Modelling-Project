using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    const float gravity = 9.82f;

    public static float r; // Radius of the bowling ball
    float m = 10.0f; // Mass of the bowling ball
    float u = 0.2f;
    float vinit = 7.15f;
    float winit;
    float alpha;
    //float omega = 8.0f; // Angular velocity, "Rotation speed", vinkelhastighet
    float vcurr; // Current velocity
    float wcurr; // Current angular velocity
    float h = 0.01f; // Step size in Euler's formula
    float a; // Acceleration
    float J; // Moment of inertia (tröghetsmoment för klot) , we can also call it I
    public static float momentum;

    public static Vector3 poscurr; // Current position of the ball
    Vector3 rotcurr; // Current rotation of the ball
    Vector3 posinit;

    float forceNormal; // Normalkraft FN
    float forceFriction; // Friktionskraft Ff

    // Start is called before the first frame update (only once)
    // Used for calculating start values and constant values
    void Start()
    {
        r = GetComponent<CircleCollider2D>().radius; // Get the radius from the game object size
        poscurr = transform.position; // Set the current ball position as the start position
        posinit = poscurr;
        rotcurr = transform.eulerAngles;
        vcurr = vinit;
        winit = vinit/r;
        wcurr = winit;
        momentum = m*vinit;

        J = (2*m*Mathf.Pow(r, 2.0f))/5; // I = (2mr^2)/5 för klot
        forceNormal = m*gravity; // FN = mg
        forceFriction = -1* u * forceNormal; // Ff = µ*FN
        alpha = (forceFriction*r)/J; // alpha = Ff*R / I
        a = forceFriction/m;
    }

    // Update is called once per frame, loops
    void Update()
    {
        if (vcurr > 0.0f) {
            momentum = m*vcurr;

            poscurr.x = poscurr.x + h*vcurr; // Calculate new position with Euler's formula
            rotcurr.z = rotcurr.z + h*wcurr;
            //rotcurr.z = ( Mathf.Abs(poscurr.x-posinit.x) /r);
            
            vcurr = vcurr + h*a; // Calculate new velocity with Euler's formula
            wcurr = wcurr + h*alpha;

            // Prevents velocity and angular velocity from becoming negative
            if (vcurr < 0.0f) {
                vcurr = 0.0f;
            }
            if (wcurr < 0.0f) {
                wcurr = 0.0f;
            }

        } else {
            vcurr = 0.0f; // Velocity is not allowed to be negative
            momentum = 0.0f;
        }
        transform.position = poscurr; // Set the object to the new position
        transform.eulerAngles = -1*rotcurr*(180/Mathf.PI);
    }
}
