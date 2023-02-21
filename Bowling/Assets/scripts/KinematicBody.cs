using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicBody : MonoBehaviour
{
    public float mass = 1;
    public float radius = 1;
    public float inertia = 1;
    public Vector3 linearVelocity = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;

    public Vector3 velocity = Vector3.zero;
}
