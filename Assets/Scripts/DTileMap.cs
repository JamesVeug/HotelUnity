using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class DTileMap : MonoBehaviour{

    // Tyile Types
    // Edit method to indicate how many in total we have 

    public int width;
    public int height;

    [SerializeField]
    private DTile[] tiles;

    BuildableRoom roomBeingConstructed;
    List<BuildableRoom> sharedRooms;
    List<BuildableRoom> rooms;
    List<BReceptionRoom> receptionRooms;
    List<BHouseKeepingRoom> houseKeepingRooms;
    List<BBedroom> bedRooms;
    public List<Vector2> changes;

    public void initialize(int width, int height)
    {
        this.width = width;
        this.height = height;

        tiles = new DTile[width* height];
        rooms = new List<BuildableRoom>();
        receptionRooms = new List<BReceptionRoom>();
        houseKeepingRooms = new List<BHouseKeepingRoom>();
        bedRooms = new List<BBedroom>();
        changes = new List<Vector2>();


        // Pathway
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[j * width + i] = DTile.GRASS();
                changes.Add(new Vector2(i, j));
            }
        }
    }

    public void setTile(int i, int j, int type)
    {
        if (tiles[j * width + i].tileType != type)
        {
            tiles[j * width + i].tileType = type;
            changes.Add(new Vector2(i, j));
        }
    }

    public int getTileType(int x, int y)
    {
        return tiles[y * width + x].tileType;
    }

    public DTile getTile(int x, int y)
    {
        return tiles[y * width + x];
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
        if( room == null)
        {
            Debug.LogWarning("No Room at " + x + "," + y);
            return null;
        }
        //Debug.Log("Room " + room.left + "," + room.top + " Walls: " + room.walls.Count);

        // Get wall from room
        return room.getWall(x, y);
    }

    public DWindow getWindow(int x, int y)
    {
        BuildableRoom room = getRoom(x, y);
        if (room == null)
        {
            return null;
        }

        // Get window from room
        return room.getWindow(x, y);
    }

    public DDoor getDoor(int x, int y)
    {
        BuildableRoom room = getRoom(x, y);
        if (room == null)
        {
            return null;
        }

        // Get door from room
        return room.getDoor(x, y);
    }

    public BuildableItem getItem(int x, int y)
    {
        BuildableRoom room = getRoom(x, y);
        if (room == null)
        {
            return null;
        }

        // Get item from room
        return room.getItem(x, y);
    }

    public BuildableRoom getRoom(int x, int y)
    {
        Vector3 pos = new Vector3(x, 0, y);

        // Check if this area is in the room being constructed
        if (roomBeingConstructed != null && roomBeingConstructed.contains(pos))
        {
            return roomBeingConstructed;
        }

        // Check other rooms
        foreach(BuildableRoom room in rooms)
        {
            if (room.contains(pos))
            {
                return room;
            }
        }

        // Could not find the room
        return null;
    }

    public void RecordRoom()
    {
        if(roomBeingConstructed is BReceptionRoom)
        {
            receptionRooms.Add((BReceptionRoom)roomBeingConstructed);
        }
        if (roomBeingConstructed is BHouseKeepingRoom)
        {
            houseKeepingRooms.Add((BHouseKeepingRoom)roomBeingConstructed);
        }

        // Bedroom Types
        if (roomBeingConstructed is BBedroom)
        {
            bedRooms.Add((BBedroom)roomBeingConstructed);
        }
        if (roomBeingConstructed is BSharedBedroom)
        {
            bedRooms.Add((BBedroom)roomBeingConstructed);
        }

        rooms.Add(roomBeingConstructed);
        roomBeingConstructed = null;
    }

    public void AddItem(BuildableItem item)
    {
        var requiredTiles = item.getItemTiles();
        foreach(Vector2 tile in requiredTiles) {
            tiles[(int)(tile.y * width + tile.x)].item = true;
            changes.Add(new Vector2((int)tile.x, (int)tile.y));
        }
    }

    public int maxTileTypes()
    {
        return 7;
    }

    public void ApplyToTileMap(BuildableRoom room)
    {
        roomBeingConstructed = room;
        for ( int i = 0; i < room.width; i++)
        {
            for (int j = 0; j < room.height; j++)
            {
                bool isNorth = j == room.height-1;
                bool isSouth = j == 0;
                bool isWest =  i == 0;
                bool isEast =  i == room.width-1;
                int x = room.left + i;
                int y = room.top + j;

                DDoor door = room.getDoor(x, y);
                DWindow window = room.getWindow(x, y);
                if (door != null)
                {
                    //Debug.Log("PlacedDoor"+x + "," + y);

                    Navigation.Direction dir = Navigation.Direction.North;
                    if (isNorth) { tiles[y * width + x].setNorthDoor(); dir = Navigation.Direction.North; }
                    if (isSouth) { tiles[y * width + x].setSouthDoor(); dir = Navigation.Direction.South; }
                    if (isWest) { tiles[y * width + x].setWestDoor(); dir = Navigation.Direction.West; }
                    if (isEast) { tiles[y * width + x].setEastDoor(); dir = Navigation.Direction.East; }
                    door.facingDirection = dir;
                }
                else if (window != null)
                {
                    //Debug.Log("PlacedWindow" + x + "," + y);

                    Navigation.Direction dir = Navigation.Direction.North;
                    if (isNorth) { tiles[y * width + x].setNorthWindow(); dir = Navigation.Direction.North; }
                    if (isSouth) { tiles[y * width + x].setSouthWindow(); dir = Navigation.Direction.South; }
                    if (isWest) { tiles[y * width + x].setWestWindow(); dir = Navigation.Direction.West; }
                    if (isEast) { tiles[y * width + x].setEastWindow(); dir = Navigation.Direction.East; }
                    window.facingDirection = dir;
                }
                else if (i == 0 || i == room.width - 1 || j == 0 || j == room.height - 1 )
                {
                    //Debug.Log("Wall " + x + "," + y);

                    Navigation.Direction dir = Navigation.Direction.North;
                    if (isNorth) { tiles[y * width + x].setNorthWall(); dir = Navigation.Direction.North; }
                    if (isSouth) { tiles[y * width + x].setSouthWall(); dir = Navigation.Direction.South; }
                    if (isWest)  { tiles[y * width + x].setWestWall();  dir = Navigation.Direction.West; }
                    if (isEast)  { tiles[y * width + x].setEastWall();  dir = Navigation.Direction.East; }
                    DWall wall = DWall.create(x,y, dir);
                    room.walls.Add(wall);
                }

                tiles[y * width + x].setRoom();
                changes.Add(new Vector2(x, y));
            }
        }
    }

    public bool hasWindow(int x, int y)
    {
        DTile tile = getTile(x, y);
        return tile.northHasWindow() || tile.westHasWindow() || tile.eastHasWindow() || tile.southHasWindow();
    }

    public bool isGrass(int x, int y)
    {
        return getTile(x, y).isGrass();
    }

    public bool isWalkWay(int x, int y)
    {
        return getTile(x, y).isWalkway();
    }

    public bool hasWall(int x, int y)
    {
        DTile tile = getTile(x, y);
        return tile.northHasWall() || tile.westHasWall() || tile.eastHasWall() || tile.southHasWall();
    }

    public bool hasDoor(int x, int y)
    {
        DTile tile = getTile(x, y);
        return tile.northHasDoor() || tile.westHasDoor() || tile.eastHasDoor() || tile.southHasDoor();
    }

    public bool hasItem(int x, int y)
    {
        return getTile(x, y).item;
    }

    public bool isRoom(int x, int y)
    {
        return getTile(x, y).isRoom();
    }

    public bool isRoom(DRectangle rect)
    {
        for(int x = rect.left; x <= rect.right; x++)
        {
            for (int y = rect.top; y <= rect.bottom; y++)
            {
                if( !getTile(x, y).isRoom() )
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool hasWall(DRectangle rect)
    {
        for (int x = rect.left; x <= rect.right; x++)
        {
            for (int y = rect.top; y <= rect.bottom; y++)
            {
                if ( !hasWall(x, y) )
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void destroy()
    {
        /*foreach(BuildableRoom r in rooms)
        {
            foreach(DWall w in r.walls)
            {
                System.Object.Destroy(w.gameObject);
            }
        }*/
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

    public List<BReceptionRoom> getReceptionRooms()
    {
        return receptionRooms;
    }

    public List<BHouseKeepingRoom> getHouseKeepingRooms()
    {
        return houseKeepingRooms;
    }

    public List<BBedroom> getBedrooms()
    {
        return bedRooms;
    }

    public void print()
    {
        for (int y = 0; y < height; y++)
        {
            string row = "";
            for (int x = 0; x < width; x++)
            {
                row = row + getTile(x, y);
            }
            Debug.Log(row);
        }
    }
}
