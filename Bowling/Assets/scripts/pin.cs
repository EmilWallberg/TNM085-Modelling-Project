using Mos.PhysicsEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pin : KinematicBody
{
    // Start is called before the first frame update
    public bool wasHit;
    void Start()
    {
        PhysicsEngine.objectsInScene.Add(gameObject);
    }

    public void Update()
    {
        if(wasHit)
        {
            
        }
    }

}
