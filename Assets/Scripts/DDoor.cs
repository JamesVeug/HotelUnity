using UnityEngine;
using System;

public class DDoor : DDataObject, IEquatable<DDoor>
{
    public Vector2 position;
    public GameObject gameObject;

    public DDoor(int x, int y)
    {
        position = new Vector2(x, y);
    }

    public DDoor(Vector2 position)
    {
        this.position = new Vector2(position.x, position.y);
    }

    public bool Equals(DDoor other)
    {
        return other !=  null && position.Equals(other.position);
    }
}
