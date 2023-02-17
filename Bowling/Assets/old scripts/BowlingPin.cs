using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    const float gravity = 9.82f;

    Vector3 position;
    float width;
    float m = 1.5f; // Bowling pin mass in kg

    float forceNormal;
    float forceFriction;
    float u = 0.2f;

    float hitPoint; // Where the ball hits the pin
    bool wasPinHit;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        width = GetComponent<BoxCollider2D>().size.x;
        hitPoint = position.x - width/2;

        // Physics equations
        forceNormal = m*gravity;
        forceFriction = forceNormal*u;
    }

    // Update is called once per frame
    void Update()
    {

        if (BowlingBall.poscurr.x+BowlingBall.r >= position.x && !wasPinHit) {
            wasPinHit = true;
            Debug.Log("Hit the pin at position " + position.x);
        }

        if (wasPinHit) {

        }
    }
}
