﻿using UnityEngine;
using System.Collections;

public static class Vector2Extension {

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static Vector2 Rotate(Vector2 position, Vector2 origin, float degrees)
    {
        return origin+Rotate(position - origin, degrees);
    }
}
