using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    [SerializeField] Transform FollowObject;
    // Update is called once per frame
    void Update()
    {
        transform.position = FollowObject.position;
    }
}
