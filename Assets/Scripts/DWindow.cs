using System;
using UnityEngine;

public class DWindow : DDataObject, IEquatable<DWindow>
{
    public Vector2 position;
    public GameObject gameObject;

    public DWindow(int x, int y)
    {
        position = new Vector2(x, y);
    }

    public DWindow(Vector2 position)
    {
        this.position = new Vector2(position.x, position.y);
    }

    public bool Equals(DWindow other)
    {
        return position.Equals(other.position);
    }
}
