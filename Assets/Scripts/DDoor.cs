using UnityEngine;
using System;

public class DDoor : DDataObject
{

    public static DDoor create(float x, float y, Navigation.Direction dir)
    {
        DDoor door = ScriptableObject.CreateInstance<DDoor>();
        door.position = new Vector2(x, y);
        door.facingDirection = dir;

        return door;
    }

    public static DDoor create(Vector2 position, Navigation.Direction dir)
    {
        DDoor door = ScriptableObject.CreateInstance<DDoor>();
        door.position = new Vector2(position.x, position.y);
        door.facingDirection = dir;

        return door;
    }
}
