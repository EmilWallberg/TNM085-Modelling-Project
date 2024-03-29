using System.Collections.Generic;
using UnityEngine;


namespace Mos.PhysicsEngine
{
    public static class PhysicsEngine
    {
        public static List<GameObject> objectsInScene = new List<GameObject>();
        public static float gravity = 9.81f;
        public static float timeStep = 0.01f;
        public static Vector3 Euler(Vector3 value, Vector3 f, float h)
        {
            return value + f * h;
        }

        public static void ImpulsAng(GameObject colidObj1, GameObject colidObj2, Vector3 collisionNormal, Vector3 collisionPoint)
        {
            KinematicBody KB1 = colidObj1.GetComponent<KinematicBody>();
            KinematicBody KB2 = colidObj2.GetComponent<KinematicBody>();

            Vector3 v1 = KB1.velocity;
            Vector3 w1 = KB1.angularVelocity;
            Vector3 v2 = KB2.velocity;
            Vector3 w2 = KB2.angularVelocity;

            float m1 = KB1.mass;
            float m2 = KB2.mass;
            float I1 = KB1.inertia;
            float I2 = KB2.inertia;

            float rotationEnergy = I1 * Mathf.Pow(w1.magnitude, 2) + I2 * Mathf.Pow(w2.magnitude, 2);
            float Energy = m1 * Mathf.Pow(v1.magnitude, 2) + m2 * Mathf.Pow(v2.magnitude, 2);

            Debug.Log("R before: " + Energy + rotationEnergy);
            float fcr = 1f;
            Vector3 vRelativeVelocity = v1 - v2;

            collisionPoint = colidObj1.transform.TransformPoint(collisionPoint);
            Vector3 vColissionPoint1 = collisionPoint - colidObj1.transform.position;
            Vector3 vColissionPoint2 = collisionPoint - colidObj2.transform.position;
            Debug.DrawLine(vColissionPoint1, vColissionPoint1 * 1.01f, Color.green);
            Debug.DrawLine(vColissionPoint2, vColissionPoint2 * 1.01f, Color.red);

            float J = (-(1 + fcr) * (Vector3.Dot(vRelativeVelocity, collisionNormal)) /
                ((1 / m1 + 1 / m2) +
                (Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(vColissionPoint1, collisionNormal) / I1, vColissionPoint1))) +
                (Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(vColissionPoint2, collisionNormal) / I2, vColissionPoint2)))
                ));
            Debug.Log(J * collisionNormal);


            float u = 0.2f;
            Vector3 T = Vector3.Cross(Vector3.Cross(vRelativeVelocity, collisionNormal), collisionNormal);
            T = T / (Vector3.Magnitude(T));

            KB1.linearVelocity = v1 + (J * collisionNormal + (u*J)*T) / m1;
            KB1.angularVelocity = w1 + (Vector3.Cross(vColissionPoint1, J * collisionNormal + (u * J) * T)) / I1;
            KB2.linearVelocity = v2 - (J * collisionNormal + (u * J) * T) / m2;
            KB2.angularVelocity = w2 - (Vector3.Cross(vColissionPoint2, J * collisionNormal + (u * J) * T)) / I2;

            rotationEnergy = I1 * Mathf.Pow(KB1.angularVelocity.magnitude, 2) + I2 * Mathf.Pow(KB2.angularVelocity.magnitude, 2);
            Energy = m1 * Mathf.Pow(v1.magnitude, 2) + m2 * Mathf.Pow(v2.magnitude, 2);
            Debug.Log("R after: " + Energy + rotationEnergy);
        }

        public static void ImpulsAngTest(GameObject colidObj1, GameObject colidObj2, Vector3 vColissionNormal, Vector3 collisionPoint, out Vector3 linearImpuls, out Vector3 angularImpuls)
        {
            Vector3 v1 = colidObj1.GetComponent<KinematicBody>().velocity;
            Vector3 v2 = colidObj2.GetComponent<KinematicBody>().velocity;

            float m1 = colidObj1.GetComponent<KinematicBody>().mass; 
            float m2 = colidObj2.GetComponent<KinematicBody>().mass;
            float I1 = colidObj1.GetComponent<KinematicBody>().inertia;
            float I2 = colidObj2.GetComponent<KinematicBody>().inertia;

            float fcr = 1f;
            Vector3 vRelativeVelocity = v1 - v2;
            Vector3 vColissionPoint1 = collisionPoint - colidObj1.transform.position;
            Vector3 vColissionPoint2 = collisionPoint - colidObj2.transform.position;
            float J = (-(1 + fcr) * (Vector3.Dot(vRelativeVelocity, vColissionNormal)) /
                ((1 / m1 + Vector3.Dot(vColissionNormal, Vector3.Cross(Vector3.Cross(vColissionPoint1, vColissionNormal) / I1, vColissionPoint1))) +
                (1 / m2 + Vector3.Dot(vColissionNormal, Vector3.Cross(Vector3.Cross(vColissionPoint2, vColissionNormal) / I2, vColissionPoint2)))));
         

            linearImpuls = J * vColissionNormal;
            angularImpuls = Vector3.Cross(vColissionPoint1, J * vColissionNormal) / I2 - Vector3.Cross(vColissionPoint1, J * vColissionNormal) / I1;
        }

        // Soruce of how to implment the GJKCollisionDetection
        // https://www.youtube.com/watch?v=ajv46BSqcK4
        public static bool GJKCollisionDetection(MeshCollider meshCollider, MeshCollider otherMeshCollider, out Vector3 collisionPoint, out Vector3 collisionNormal)
        {
            Transform meshTransform = meshCollider.transform;
            Transform otherMeshTransform = otherMeshCollider.transform;
            collisionPoint = collisionNormal = Vector3.zero;

            Vector3[] vertices = new Vector3[meshCollider.sharedMesh.vertices.Length];
            Vector3[] otherVertices = new Vector3[otherMeshCollider.sharedMesh.vertices.Length];

            //Move the vertices from local to world space
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = meshTransform.TransformPoint(meshCollider.sharedMesh.vertices[i]);
            }
            for (int i = 0; i < otherVertices.Length; i++)
            {
                otherVertices[i] = otherMeshTransform.TransformPoint(otherMeshCollider.sharedMesh.vertices[i]);
            }


            Vector3[] supportPoints = new Vector3[4];
            int simplexCount = 0;
            Vector3 direction = Vector3.one;

            // Set an initial point to be the difference between the two object's centers
            Vector3 initialPoint = meshTransform.position - otherMeshTransform.position;

            // Get the support point in the direction of the initial point
            supportPoints[0] = Support(vertices, otherVertices, direction);
            direction = -initialPoint;

            while (true)
            {
                // Get the support point in the direction of the current search direction
                supportPoints[++simplexCount] = Support(vertices, otherVertices, direction);

                // If the new point did not pass the origin, there is no collision
                if (Vector3.Dot(supportPoints[simplexCount], direction) <= 0)
                {
                    return false;
                }

                // Check if the simplex contains the origin
                if (CheckSimplex(ref supportPoints, ref simplexCount, ref direction))
                {
                    collisionNormal = -direction.normalized;

                    // Calculate the closest point on the simplex to the origin
                    if (simplexCount == 2)
                    {
                        collisionPoint = ClosestPointOnLineSegment(Vector3.zero, supportPoints[1], supportPoints[0]);
                    }
                    else if (simplexCount == 3)
                    {
                        collisionPoint = ClosestPointOnTriangle(Vector3.zero, supportPoints[2], supportPoints[1], supportPoints[0]);
                    }
                    else // simplexCount == 4
                    {
                        collisionPoint = ClosestPointOnTetrahedron(Vector3.zero, supportPoints[3], supportPoints[2], supportPoints[1], supportPoints[0]);
                    }

                    return true;
                }
            }
        }

        private static bool CheckSimplex(ref Vector3[] simplex, ref int count, ref Vector3 direction)
        {
            if (count == 2)
            {
                Vector3 a = simplex[1];
                Vector3 b = simplex[0];
                Vector3 ab = b - a;

                // Search perpendicular to AB towards the origin
                Vector3 ao = -a;
                direction = Vector3.Cross(ab, Vector3.Cross(ao, ab));

                return false;
            }

            else if (count == 3)
            {
                Vector3 a = simplex[2];
                Vector3 b = simplex[1];
                Vector3 c = simplex[0];

                Vector3 ab = b - a;
                Vector3 ac = c - a;

                // Check if the origin is inside the triangle
                Vector3 abc = Vector3.Cross(ab, ac);
                if (Vector3.Dot(Vector3.Cross(abc, ac), a) > 0)
                {
                    if (Vector3.Dot(ac, -a) > 0)
                    {
                        // Remove b, search perpendicular to AC towards the origin
                        direction = Vector3.Cross(ac, Vector3.Cross(-a, ac));
                        simplex[1] = simplex[2];
                        count--;
                    }
                    else
                    {
                        // Remove a, search perpendicular to AB towards the origin
                        direction = Vector3.Cross(ab, Vector3.Cross(-a, ab));
                        simplex[0] = simplex[1];
                        simplex[1] = simplex[2];
                        count--;
                    }
                    return false;
                }

                // Check if the origin is inside the edge AB
                if (Vector3.Dot(ab, -a) > 0)
                {
                    direction = Vector3.Cross(ab, Vector3.Cross(-a, ab));
                    simplex[0] = simplex[1];
                    simplex[1] = simplex[2];
                    count--;
                    return false;
                }

                // Check if the origin is inside the edge AC
                if (Vector3.Dot(ac, -a) > 0)
                {
                    direction = Vector3.Cross(ac, Vector3.Cross(-a, ac));
                    simplex[1] = simplex[2];
                    count--;
                    return false;
                }

                // The origin is inside the triangle
                direction = abc.normalized;
                return true;
            }
            else
            {
                // count == 4
                Vector3 a = simplex[3];
                Vector3 b = simplex[2];
                Vector3 c = simplex[1];
                Vector3 d = simplex[0];

                Vector3 ab = b - a;
                Vector3 ac = c - a;
                Vector3 ad = d - a;

                Vector3 abc = Vector3.Cross(ab, ac);
                if (Vector3.Dot(abc, ad) > 0)
                {
                    // Remove d, search perpendicular to ABC towards the origin
                    direction = abc.normalized;
                    simplex[0] = a;
                    simplex[1] = b;
                    simplex[2] = c;
                    count--;
                }
                else
                {
                    Vector3 acd = Vector3.Cross(ac, ad);
                    if (Vector3.Dot(acd, ab) > 0)
                    {
                        // Remove b, search perpendicular to ACD towards the origin
                        direction = acd.normalized;
                        simplex[1] = a;
                        simplex[2] = c;
                        count--;
                    }
                    else
                    {
                        Vector3 adb = Vector3.Cross(ad, ab);
                        if (Vector3.Dot(adb, ac) > 0)
                        {
                            // Remove c, search perpendicular to ADB towards the origin
                            direction = adb.normalized;
                            simplex[2] = a;
                            count--;
                        }
                        else
                        {
                            // The origin is inside the tetrahedron
                            return true;
                        }
                    }
                }

                return false;
            }
        }


        private static Vector3 Support(Vector3[] vertices, Vector3[] otherVertices, Vector3 direction)
        {
            Vector3 support1 = GetFurthestPointInDirection(vertices, direction);
            Vector3 support2 = GetFurthestPointInDirection(otherVertices, -direction);
            return support1 - support2;
        }

        private static Vector3 GetFurthestPointInDirection(Vector3[] vertices, Vector3 direction)
        {
            int index = 0;
            float maxDot = Vector3.Dot(vertices[0], direction);
            for (int i = 1; i < vertices.Length; i++)
            {
                float dot = Vector3.Dot(vertices[i], direction);
                if (dot > maxDot)
                {
                    index = i;
                    maxDot = dot;
                }
            }

            return vertices[index];
        }

        private static Vector3 ClosestPointOnLineSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            float t = Vector3.Dot(p - a, ab) / Vector3.Dot(ab, ab);
            t = Mathf.Clamp01(t);
            return a + t * ab;
        }

        private static Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = p - a;
            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);
            if (d1 <= 0f && d2 <= 0f)
            {
                return a;
            }

            Vector3 bp = p - b;
            float d3 = Vector3.Dot(ab, bp);
            float d4 = Vector3.Dot(ac, bp);
            if (d3 >= 0f && d4 <= d3)
            {
                return b;
            }

            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0f && d1 >= 0f && d3 <= 0f)
            {

                Vector3 cp = ClosestPointOnLineSegment(p, a, b);
                return cp;
            }

            Vector3 cp2;
            if (d2 >= 0f && d4 >= 0f && (d3 - d4) <= d2 - d1)
            {
                cp2 = ClosestPointOnLineSegment(p, b, c);
                return cp2;
            }

            float vb = d3 * d2 - d1 * d4;
            if (vb <= 0f && d2 >= 0f && d4 <= 0f)
            {
                cp2 = ClosestPointOnLineSegment(p, a, c);
                return cp2;
            }

            float va = vc + vb;
            float denom = 1f / (va + vb + vc);
            float u = va * denom;
            float v = vb * denom;
            float w = 1f - u - v;

            Vector3 closestPointOnTriangle = u * a + v * b + w * c;
            return closestPointOnTriangle;

        }

        private static Vector3 ClosestPointOnTetrahedron(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 closestPointOnTriangleABC = ClosestPointOnTriangle(p, a, b, c);
            Vector3 closestPointOnTriangleABD = ClosestPointOnTriangle(p, a, b, d);
            Vector3 closestPointOnTriangleACD = ClosestPointOnTriangle(p, a, c, d);
            Vector3 closestPointOnTriangleBCD = ClosestPointOnTriangle(p, b, c, d);

            float distanceToABC = Vector3.Distance(p, closestPointOnTriangleABC);
            float distanceToABD = Vector3.Distance(p, closestPointOnTriangleABD);
            float distanceToACD = Vector3.Distance(p, closestPointOnTriangleACD);
            float distanceToBCD = Vector3.Distance(p, closestPointOnTriangleBCD);

            float minDistance = Mathf.Min(distanceToABC, distanceToABD, distanceToACD, distanceToBCD);

            if (minDistance == distanceToABC)
            {
                return closestPointOnTriangleABC;
            }
            else if (minDistance == distanceToABD)
            {
                return closestPointOnTriangleABD;
            }
            else if (minDistance == distanceToACD)
            {
                return closestPointOnTriangleACD;
            }
            else // minDistance == distanceToBCD
            {
                return closestPointOnTriangleBCD;
            }
        }
    }
}