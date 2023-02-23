using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mos.PhysicsEngine
{

    
    public static class PhysicsEngine
    {
      
        public static List<GameObject> objectsInScene=new List<GameObject>();
        public static float gravity = 9.81f;
        public static Vector3 Euler(Vector3 value, Vector3 f, float h)
        {
            return value + f * h;
        }

        public static bool CheckCollision(Mesh mesh, Mesh otherMesh, out Vector3 collisionNormal)
        {
            collisionNormal = Vector3.zero;
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                for (int j = 0; j < otherMesh.triangles.Length; j += 3)
                {
                    // Check if the triangles intersect
                    if (TriangleIntersectsTriangle(
                            mesh.vertices[mesh.triangles[i]],
                            mesh.vertices[mesh.triangles[i + 1]],
                            mesh.vertices[mesh.triangles[i + 2]],
                            otherMesh.vertices[otherMesh.triangles[j]],
                            otherMesh.vertices[otherMesh.triangles[j + 1]],
                            otherMesh.vertices[otherMesh.triangles[j + 2]],
                            out collisionNormal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TriangleIntersectsTriangle(
            Vector3 v1, Vector3 v2, Vector3 v3,
            Vector3 u1, Vector3 u2, Vector3 u3,
            out Vector3 collisionNormal)
        {
            // Use the Separating Axis Theorem to check if the triangles intersect
            Vector3[] triangle1 = { v1, v2, v3 };
            Vector3[] triangle2 = { u1, u2, u3 };

            collisionNormal = Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                Vector3 axis = Vector3.Cross(triangle1[i] - triangle1[(i + 1) % 3], triangle1[i] - triangle1[(i + 2) % 3]);
                if (SeparatingAxis(axis, triangle1, triangle2))
                {
                    return false;
                }

                axis = Vector3.Cross(triangle2[i] - triangle2[(i + 1) % 3], triangle2[i] - triangle2[(i + 2) % 3]);
                if (SeparatingAxis(axis, triangle1, triangle2))
                {
                    return false;
                }

                // Calculate the collision normal
                axis = Vector3.Cross(triangle1[i] - triangle1[(i + 1) % 3], triangle1[i] - triangle1[(i + 2) % 3]).normalized;
                float distance = Vector3.Dot(triangle1[i], axis);
                float otherDistance = Vector3.Dot(triangle2[0], axis);
                for (int j = 1; j < 3; j++)
                {
                    float d = Vector3.Dot(triangle2[j], axis);
                    if (d < otherDistance)
                    {
                        otherDistance = d;
                    }
                }
                float penetration = distance - otherDistance;
                if (penetration > 0)
                {
                    collisionNormal += axis * penetration;
                }
            }

            collisionNormal = collisionNormal.normalized;
            return true;
        }

        // This algorithm checks if there is a separating axis between the two triangles
        // If there is no separating axis, the triangles must intersect
        private static bool SeparatingAxis(Vector3 axis, Vector3[] triangle1, Vector3[] triangle2)
        {
            // Project the triangles onto the axis and check if there is a gap between the projections
            float min1 = float.MaxValue;
            float max1 = float.MinValue;
            float min2 = float.MaxValue;
            float max2 = float.MinValue;

            for (int i = 0; i < 3; i++)
            {
                float dot1 = Vector3.Dot(axis, triangle1[i]);
                if (dot1 < min1)
                {
                    min1 = dot1;
                }
                if (dot1 > max1)
                {
                    max1 = dot1;
                }

                float dot2 = Vector3.Dot(axis, triangle2[i]);
                if (dot2 < min2)
                {
                    min2 = dot2;
                }
                if (dot2 > max2)
                {
                    max2 = dot2;
                }
            }

            if (min1 > max2 || min2 > max1)
            {
                return true;
            }

            return false;
        }


        //list[0] = nya v1, //list[1] = nya w1,  //list[2] = nya v2, //list[3] = nya w2, 
        public static List<Vector3> ImpulsAng(GameObject colidObj1, GameObject colidObj2)
        {
            List<Vector3> listOfVel = new List<Vector3>();

            Vector3 v1 = colidObj1.GetComponent<KinematicBody>().velocity;
            Vector3 w1 = colidObj1.GetComponent<KinematicBody>().angularVelocity;
            Vector3 v2 = colidObj2.GetComponent<KinematicBody>().velocity;
            Vector3 w2 = colidObj2.GetComponent<KinematicBody>().angularVelocity;

            float m1 = colidObj1.GetComponent<KinematicBody>().mass;
            float m2 = colidObj2.GetComponent<KinematicBody>().mass;
            float I1 = colidObj1.GetComponent<KinematicBody>().inertia;
            float I2 = colidObj2.GetComponent<KinematicBody>().inertia;
            Vector3 pos1 = colidObj1.transform.position;
            Vector3 pos2 = colidObj2.transform.position;

            //Temp
            Vector3 collisionPoint = colidObj1.transform.position;
            collisionPoint.x += colidObj1.GetComponent<KinematicBody>().radius;


            float fcr = 1;
            Vector3 vRelativeVelocity = v1 - v2;
            Vector3 vColissionNormal = pos1 - pos2;
            vColissionNormal = vColissionNormal.normalized;
            Vector3 vColissionPoint1 = collisionPoint - colidObj1.transform.position;
            Vector3 vColissionPoint2 = collisionPoint - colidObj2.transform.position;
            //Vector3.Dot(v1, v2);
            //Vector3.Cross(v1, v2);
            float J = (-(1 + fcr) * (Vector3.Dot(vRelativeVelocity, vColissionNormal)) /
                ((1 / m1 + 1 / m2) +
                (Vector3.Dot(vColissionNormal, Vector3.Cross(Vector3.Cross(vColissionPoint1, vColissionNormal) / I1, vColissionPoint1))) +
                (Vector3.Dot(vColissionNormal, Vector3.Cross(Vector3.Cross(vColissionPoint2, vColissionNormal) / I2, vColissionPoint2)))
                ));

            Vector3 newV1 = v1 + (J * vColissionNormal) / m1;
            Vector3 newW1 = w1 + (Vector3.Cross(vColissionPoint1, J * vColissionNormal)) / m1;
            Vector3 newV2 = v2 + (J * vColissionNormal) / m2; ;
            Vector3 newW2 = w2 + (Vector3.Cross(vColissionPoint2, J * vColissionNormal)) / m2;

            listOfVel.Add(newV1); listOfVel.Add(newW1); listOfVel.Add(newV2); listOfVel.Add(newW2);

            return listOfVel;
        }

        //list[0] = nya v1, //list[1] = nya v2,
        public static List<Vector3> Impuls(GameObject colidObj1, GameObject colidObj2)
        {
            List<Vector3> listOfAngV = new List<Vector3>();

            float r = colidObj1.GetComponent<KinematicBody>().radius + colidObj1.GetComponent<KinematicBody>().radius;
            Vector3 d = colidObj1.transform.position - colidObj2.transform.position;
            float s = d.magnitude - r;

            d.Normalize();
            Vector3 vCollisionNormal = d;

            Vector3 v1 = colidObj1.GetComponent<KinematicBody>().velocity;
            Vector3 v2 = colidObj2.GetComponent<KinematicBody>().velocity;
            float m1 = colidObj1.GetComponent<KinematicBody>().mass;
            float m2 = colidObj2.GetComponent<KinematicBody>().mass;

            Vector3 vRelativeVelocity = v1 - v2;

            float fcr = 1;
            float Vrn = Vector3.Dot(vRelativeVelocity, vCollisionNormal);

            float J = (-(1 + fcr) * (Vector3.Dot(vRelativeVelocity, vCollisionNormal)) / ((Vector3.Dot(vCollisionNormal, vCollisionNormal)) * (1 / m1 + 1 / m2)));

            Vector3 newV1 = v1 + (J * vCollisionNormal) / m1;
            Vector3 newV2 = v1 - (J * vCollisionNormal) / m2;

            listOfAngV.Add(newV1); listOfAngV.Add(newV2);

            return listOfAngV;

        }
    }


}