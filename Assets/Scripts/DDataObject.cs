using UnityEngine;
using System;

public abstract class DDataObject : ScriptableObject , IEquatable<DDataObject>{
    public Navigation.Direction facingDirection;
    public Vector2 position;
    public GameObject gameObject;

    public bool Equals(DDataObject other)
    {
        return other != null && position.Equals(other.position);
    }

    public Vector2 nextPosition()
    {
        if( facingDirection == Navigation.Direction.North)
        {
            return new Vector2(0, 1) + position;
        }
        else if (facingDirection == Navigation.Direction.East)
        {
            return new Vector2(1, 0) + position;
        }
        else if (facingDirection == Navigation.Direction.West)
        {
            return new Vector2(-1, 0) + position;
        }
        else if (facingDirection == Navigation.Direction.South)
        {
            return new Vector2(0, -1) + position;
        }

        Debug.Log("Unknown direction " + facingDirection);
        return position;
    }
}
