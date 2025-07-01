using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    public static Vector3 AimPosY(this Ray ray, float y)
    {
        float t = (ray.origin.y - y) / ray.direction.y * -1.0f;
        Vector3 tmp = ray.origin + t * ray.direction;
        tmp.y = 0;
        return tmp;
    }
}