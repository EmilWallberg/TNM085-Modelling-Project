using Mos.PhysicsEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pin : KinematicBody
{
    // Start is called before the first frame update

 

    void Start()
    {
        base.Start();
        inertia = mass * Mathf.Pow(radius, 2.0f)/2;
    }

    public void Update()
    {
        base.Update();
 
    }

}
