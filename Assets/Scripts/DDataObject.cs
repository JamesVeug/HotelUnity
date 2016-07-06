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
}
