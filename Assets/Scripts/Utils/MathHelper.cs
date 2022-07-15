using UnityEngine;
public static class MathHelper
{
    public static float VesicaPiscisArea(float r)
    {
        return 1.28884f * (r * r);
    }
    public static Vector3 GetPointAtHeight(Ray ray, float height)
    {
        return ray.origin + (((ray.origin.y - height) / -ray.direction.y) * ray.direction);
    }
}
