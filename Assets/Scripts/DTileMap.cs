using UnityEngine;
using System.Collections.Generic;

public class DTileMap  {

    public const int GRASS = 0;
    public const int ROOM = 1;
    public const int WALL = 2;
    public const int DOOR = 3;
    public const int WINDOW = 4;
    public const int ITEM = 5;

    public int width;
    public int height;
    int[,] tiles;
    List<BuildableRoom> rooms;
    public List<Vector2> changes;

    public DTileMap(int width, int height)
    {

        this.width = width;
        this.height = height;

        tiles = new int[width, height];
        rooms = new List<BuildableRoom>();
        changes = new List<Vector2>();

        /*int randomRooms = 5;
        while(rooms.Count < randomRooms)
        {
            int rsx = Random.Range(4, 12);
            int rsy = Random.Range(4, 12);
            BuildableRoom room = new BuildableRoom(
                Random.Range(0, width - rsx),
                Random.Range(0, height - rsy),
                rsx,
                rsy);

            if (!collides(room)) {
                rooms.Add(room);
                MakeRoom(room);
            }
        }

        foreach(BuildableRoom r in rooms)
        {
            DWall wall;
            bool horizontal = Random.Range(0, 1) == 0;
            int x;
            int y;
            if( horizontal)
            {
                x = (int)(Random.Range(1, r.width-1) + r.position.x);
                y = (int)(Random.Range(0, 1)*(r.height-1) + r.position.z);
                wall = r.getWall(x, y);
            }
            else
            {
                x = (int)(Random.Range(0, 1) * (r.width-1) + r.position.x);
                y = (int)(Random.Range(1, r.height-1) + r.position.z);
                wall = r.getWall(x, y);
            }
            
            DDoor door = new DDoor(x, y);
            r.doors.Add(door);
            tiles[x, y] = DOOR;

            if (wall != null)
            {
                Object.Destroy(wall.gameObject);
            }
            r.walls.Remove(wall);
        }*/

        changes.Clear();
    }

    public int getTile(int x, int y)
    {
        return tiles[x, y];
    }

    public bool collides(DRectangle other)
    {
        foreach (BuildableRoom room in rooms)
        {
            if( room.collidesWith(other))
            {
                return true;
            }
        }
        return false;
    }

    public DWall getWall(int x, int y)
    {
        BuildableRoom room = getRoom(x, y);
        return room.getWall(x, y);
    }

    public DWindow getWindow(int x, int y)
    {
        BuildableRoom room = getRoom(x, y);
        return room.getWindow(x, y);
    }

    public DDoor getDoor(int x, int y)
    {
        BuildableRoom room = getRoom(x, y);
        return room.getDoor(x, y);
    }

    public BuildableItem getItem(int x, int y)
    {
        BuildableRoom room = getRoom(x, y);
        return room.getItem(x, y);
    }

    public BuildableRoom getRoom(int x, int y)
    {
        Vector3 pos = new Vector3(x, 0, y);
        foreach(BuildableRoom room in rooms)
        {
            if (room.contains(pos))
            {
                return room;
            }
        }
        return null;
    }

    public BuildableRoom MakeRoom(BuildableRoom room)
    {
        //Debug.Log("d " + l + " " + t + " " + w + " " + h);
        /*List<DDoor> doors = new List<DDoor>();
        List<DWindow> windows = new List<DWindow>();

        foreach (DDoor v in room.doors)
        {
            doors.Add(v);
        }
        foreach (DWindow v in room.windows)
        {
            windows.Add(v);
        }*/


        if (!collides(room))
        {
            rooms.Add(room);
            ApplyToTileMap(room);
        }
        return room;
    }

    public void AddItem(BuildableItem item)
    {
        var requiredTiles = item.getTiles();
        foreach(Vector2 tile in requiredTiles) {
            tiles[(int)tile.x, (int)tile.y] = ITEM;
            changes.Add(new Vector2((int)tile.x, (int)tile.y));
        }
    }

    void ApplyToTileMap(BuildableRoom room)
    {
        for ( int i = 0; i < room.width; i++)
        {
            for (int j = 0; j < room.height; j++)
            {
                int x = room.left + i;
                int y = room.top + j;

                if (room.getDoor(x, y) != null)
                {
                    Debug.Log("PlacedDoor"+x + "," + y);
                    tiles[x, y] = DOOR;
                }
                else if ( room.getWindow(x, y) != null)
                {
                    Debug.Log("PlacedWindow" + x + "," + y);
                    tiles[x, y] = WINDOW;
                }
                else if (i == 0 || i == room.width - 1 || j == 0 || j == room.height - 1 )
                {
                    //Debug.Log(x + "," + y);
                    tiles[x,y] = WALL;
                    DWall wall = new DWall(x,y);
                    room.walls.Add(wall);
                }
                else {
                    tiles[x,y] = ROOM;
                }
                changes.Add(new Vector2(x, y));
            }
        }
    }

    public bool isWindow(int x, int y)
    {
        return tiles[x, y] == WINDOW;
    }

    public bool isWall(int x, int y)
    {
        return tiles[x, y] == WALL;
    }

    public bool isDoor(int x, int y)
    {
        return tiles[x, y] == DOOR;
    }

    public bool isItem(int x, int y)
    {
        return tiles[x, y] == ITEM;
    }

    public bool isRoom(int x, int y)
    {
        return tiles[x, y] == ROOM;
    }

    public bool isRoom(DRectangle rect)
    {
        for(int x = rect.left; x <= rect.right; x++)
        {
            for (int y = rect.top; y <= rect.bottom; y++)
            {
                if( tiles[x,y] != ROOM)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void destroy()
    {
        foreach(BuildableRoom r in rooms)
        {
            foreach(DWall w in r.walls)
            {
                Object.Destroy(w.gameObject);
            }
        }
    }

    public List<DWall> getWalls()
    {
        List<DWall> walls = new List<DWall>();
        foreach(BuildableRoom r in rooms)
        {
            foreach (DWall w in r.walls)
            {
                walls.Add(w);
            }
        }

        return walls;
    }

    public List<DDoor> getDoors()
    {
        List<DDoor> doors = new List<DDoor>();
        foreach (BuildableRoom r in rooms)
        {
            foreach (DDoor w in r.doors)
            {
                doors.Add(w);
            }
        }

        return doors;
    }

    public List<DWindow> getWindows()
    {
        List<DWindow> windows = new List<DWindow>();
        foreach (BuildableRoom r in rooms)
        {
            foreach (DWindow w in r.windows)
            {
                windows.Add(w);
            }
        }

        return windows;
    }

    public void print()
    {
        for (int y = 0; y < height; y++)
        {
            string row = "";
            for (int x = 0; x < width; x++)
            {
                row = row + tiles[x, y];
            }
            Debug.Log(row);
        }
    }
}
