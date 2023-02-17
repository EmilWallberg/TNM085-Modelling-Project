using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicBody : MonoBehaviour
{
    [SerializeField]
    protected float mass = 1;
    [SerializeField]
    protected Vector3 linearVelocity = Vector3.zero;
    [SerializeField]
    protected Vector3 angularVelocity = Vector3.zero;

    protected Vector3 velocity = Vector3.zero;
}
