using System;
using UnityEngine;

public class DWindow : DDataObject
{
    public static DWindow create(float x, float y, Navigation.Direction dir)
    {
        DWindow window = ScriptableObject.CreateInstance<DWindow>();
        window.position = new Vector2(x, y);
        window.facingDirection = dir;

        return window;
    }

    public static DWindow create(Vector2 position, Navigation.Direction dir)
    {
        DWindow window = ScriptableObject.CreateInstance<DWindow>();
        window.position = new Vector2(position.x, position.y);
        window.facingDirection = dir;

        return window;
    }
}
