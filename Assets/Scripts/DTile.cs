using UnityEngine;
using System.Collections;
using System;

public class DTile : ScriptableObject
{
    public const int TYPE_WALKWAY = 0;
    public const int TYPE_GRASS = 1;
    public const int TYPE_ROOM = 2;

    public const int WALL_TYPE_NULL = 0;
    public const int WALL_TYPE_WALL = 1;
    public const int WALL_TYPE_DOOR = 2;
    public const int WALL_TYPE_WINDOW = 3;

    public int northWall = 0;
    public int eastWall = 0;
    public int westWall = 0;
    public int southWall = 0;

    public int tileType = 0;
    public bool item = false;



    public static DTile WALKWAY()
    {
        DTile t = ScriptableObject.CreateInstance<DTile>();
        t.tileType = TYPE_WALKWAY;
        return t;
    }

    public static DTile GRASS()
    {
        DTile t = ScriptableObject.CreateInstance<DTile>();
        t.tileType = TYPE_GRASS;
        return t;
    }

    public static DTile ROOM()
    {
        DTile t = ScriptableObject.CreateInstance<DTile>();
        t.tileType = TYPE_ROOM;
        return t;
    }

    public bool isGrass()
    {
        return tileType == TYPE_GRASS;
    }

    public bool isRoom()
    {
        return tileType == TYPE_ROOM;
    }

    public bool isWalkway()
    {
        return tileType == TYPE_WALKWAY;
    }

    public bool northHasDoor()
    {
        return northWall == WALL_TYPE_DOOR;
    }

    public bool northHasWall()
    {
        return northWall == WALL_TYPE_WALL;
    }

    public bool northHasWindow()
    {
        return northWall == WALL_TYPE_WINDOW;
    }

    public bool eastHasDoor()
    {
        return eastWall == WALL_TYPE_DOOR;
    }

    public bool eastHasWall()
    {
        return eastWall == WALL_TYPE_WALL;
    }

    public bool eastHasWindow()
    {
        return eastWall == WALL_TYPE_WINDOW;
    }

    public bool westHasDoor()
    {
        return westWall == WALL_TYPE_DOOR;
    }

    public bool westHasWall()
    {
        return westWall == WALL_TYPE_WALL;
    }

    public bool westHasWindow()
    {
        return westWall == WALL_TYPE_WINDOW;
    }

    public bool southHasDoor()
    {
        return southWall == WALL_TYPE_DOOR;
    }

    public bool southHasWall()
    {
        return southWall == WALL_TYPE_WALL;
    }

    public bool southHasWindow()
    {
        return southWall == WALL_TYPE_WINDOW;
    }

    public void setWalkWay()
    {
        tileType = TYPE_WALKWAY;
    }

    public void setGrass()
    {
        tileType = TYPE_GRASS;
    }

    public void setRoom()
    {
        tileType = TYPE_ROOM;
    }

    public void setNorthWall()
    {
        northWall = WALL_TYPE_WALL;
    }

    public void setNorthWindow()
    {
        northWall = WALL_TYPE_WINDOW;
    }

    public void setNorthDoor()
    {
        northWall = WALL_TYPE_DOOR;
    }

    public void setEastWall()
    {
        eastWall = WALL_TYPE_WALL;
    }

    public void setEastWindow()
    {
        eastWall = WALL_TYPE_WINDOW;
    }

    public void setEastDoor()
    {
        eastWall = WALL_TYPE_DOOR;
    }

    public void setWestWall()
    {
        westWall = WALL_TYPE_WALL;
    }

    public void setWestWindow()
    {
        westWall = WALL_TYPE_WINDOW;
    }

    public void setWestDoor()
    {
        westWall = WALL_TYPE_DOOR;
    }

    public void setSouthWall()
    {
        southWall = WALL_TYPE_WALL;
    }

    public void setSouthWindow()
    {
        southWall = WALL_TYPE_WINDOW;
    }

    public void setSouthDoor()
    {
        southWall = WALL_TYPE_DOOR;
    }

    public bool hasItem()
    {
        return item;
    }

    public void drawDebugTile(Vector2 position, float size, Color color, float time)
    {
        Vector3 pos = new Vector3(position.x, 0, position.y);

        Vector3 topleft = pos + new Vector3(0,0,size);
        Vector3 topright = pos + new Vector3(size, 0, size);
        Vector3 bottomright = pos + new Vector3(size, 0, 0);
        Vector3 bottomleft = pos;

        // Outline
        Debug.DrawLine(bottomleft, bottomright, color, time);
        Debug.DrawLine(bottomright, topright, color, time);
        Debug.DrawLine(topright, topleft, color, time);
        Debug.DrawLine(topleft, bottomleft, color, time);

        // Cross
        Debug.DrawLine(bottomleft, topright, color, time);
        Debug.DrawLine(topleft, bottomright, color, time);

        // Walls
        if (northHasWall() || northHasWindow()) drawDebugWall(topleft,topright,size,color,time);
        if (eastHasWall() || eastHasWindow())   drawDebugWall(bottomright, topright, size, color, time);
        if (westHasWall() || westHasWindow())   drawDebugWall(topleft, bottomleft, size, color, time);
        if (southHasWall() || southHasWindow()) drawDebugWall(bottomright, bottomleft, size, color, time);
    }

    private void drawDebugWall(Vector3 position1, Vector3 position2, float size, Color color, float time)
    {
        Vector3 heightOffset = new Vector3(0, 3, 0);
        Vector3 upperLeft = position1 + heightOffset;
        Vector3 upperRight = position2 + heightOffset;

        // Outline
        Debug.DrawLine(position1, upperLeft, color, time);
        Debug.DrawLine(position2, upperRight, color, time);
        Debug.DrawLine(upperLeft, upperRight, color, time);

        // Cross
        Debug.DrawLine(upperRight, position1, color, time);
        Debug.DrawLine(upperLeft, position2, color, time);
    }
}
