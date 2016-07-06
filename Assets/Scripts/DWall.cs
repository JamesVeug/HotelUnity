using UnityEngine;

public class DWall : DDataObject
{

    public static DWall create(float x, float y, Navigation.Direction dir)
    {
        DWall wall = ScriptableObject.CreateInstance<DWall>();
        wall.position = new Vector2(x, y);
        wall.facingDirection = dir;

        return wall;
    }

    public static DWall create(Vector2 position, Navigation.Direction dir)
    {
        DWall wall = ScriptableObject.CreateInstance<DWall>();
        wall.position = new Vector2(position.x, position.y);
        wall.facingDirection = dir;

        return wall;
    }
}
