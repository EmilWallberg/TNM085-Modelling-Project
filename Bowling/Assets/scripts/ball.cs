using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicsEngine;

public class ball : KinematicBody
{
    float radius = 2;
    // Update is called once per frame
    void Update()
    {
        
    }

    

    GameObject CheckCollision(GameObject[] gameObjects) { 
        foreach(GameObject go in gameObjects)
        {
            float distance = Vector3.Distance(this.transform.position, go.transform.position);
            if (distance <= radius)
            {

            }
    }
}
