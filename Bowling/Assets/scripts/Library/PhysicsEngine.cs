using UnityEngine;

namespace Mos.PhysicsEngine
{
    public static class PhysicsEngine
    {
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
    }
}