﻿using UnityEngine;
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
    public bool buildHotel = true;

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
                addChange(new Vector2(i, j));
            }
        }

        // Build Temporary Hotel
        if( buildHotel)
        {
            buildTempHotel();
        }
    }

    private void buildTempHotel()
    {
        Debug.Log("Building Hotel");

        // Reception 2,15,7,8 Doors 7,22 & 2,16
        // FrontDesk 3,20
        List<DDoor> receptionDoors = new List<DDoor>();
        receptionDoors.Add(DDoor.create(new Vector2(7, 22), Navigation.Direction.North));
        receptionDoors.Add(DDoor.create(new Vector2(2, 16), Navigation.Direction.West));
        BReceptionRoom reception = BReceptionRoom.Create(2, 15, 7, 8, 3, 19, 0, receptionDoors);
        AddItem(reception.frontdesks[0]);
        ApplyToTileMap(reception);
        RecordRoom();


        // Bedroom 9,15,9,8 Doors 10,22 & 16,22
        // beds 10-16,20
        List<DDoor> bedroomDoors = new List<DDoor>();
        bedroomDoors.Add(DDoor.create(new Vector2(10, 22), Navigation.Direction.North));
        bedroomDoors.Add(DDoor.create(new Vector2(16, 22), Navigation.Direction.North));
        List<BuildableBed> beds = new List<BuildableBed>();
        for(int i = 0; i <= 6; i++)
        {
            beds.Add(ISingleBed.create(new Vector2(10+i, 20), 0));
            beds.Add(ISingleBed.create(new Vector2(10 + i, 17), 0));
        }

        BSharedBedroom bedroom = BSharedBedroom.Create(9, 15, 9, 8, bedroomDoors, beds);
        ApplyToTileMap(bedroom);
        for (int i = 0; i < bedroom.bedCount(); i++)
        {
            BuildableBed bed = bedroom.getBed(i);
            AddItem(bed);
        }
        RecordRoom();


        // HK 18,15,5,8 Doors 19,22
        List<DDoor> hkDoors = new List<DDoor>();
        hkDoors.Add(DDoor.create(new Vector2(19, 22), Navigation.Direction.North));
        BHouseKeepingRoom houseKeeping = BHouseKeepingRoom.Create(18, 15, 5, 8, hkDoors);
        ApplyToTileMap(houseKeeping);
        RecordRoom();


    }

    public void setTile(int i, int j, int type)
    {
        if (tiles[j * width + i].tileType != type)
        {
            tiles[j * width + i].tileType = type;
            addChange(new Vector2(i, j));
        }
    }

    public void addChange(Vector2 v)
    {
        if( !changes.Contains(v))
        {
            changes.Add(v);
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
            //Debug.Log("Room doesn't exist " + x + "," + y);
            return null;
        }

        // Get door from room
        return room.getDoor(x, y);
    }

    public BuildableItem getItem(int x, int y)
    {
        // Get item from room
        return getItem(x, y,getRoom(x,y));
    }

    public BuildableItem getItem(int x, int y, BuildableRoom room)
    {
        if (room == null)
        {
            Debug.Log("Room does not exist " + x + "," + y);
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

    public void RemoveRoom(BuildableRoom room)
    {
        if (room is BReceptionRoom)
        {
            receptionRooms.Remove((BReceptionRoom)room);
        }
        if (room is BHouseKeepingRoom)
        {
            houseKeepingRooms.Remove((BHouseKeepingRoom)room);
        }

        // Bedroom Types
        if (room is BBedroom)
        {
            bedRooms.Remove((BBedroom)room);
        }

        rooms.Remove(room);
    }

    public void AddItem(BuildableItem item)
    {
        var requiredTiles = item.getItemTiles();
        foreach(Vector2 tile in requiredTiles) {
            tiles[(int)(tile.y * width + tile.x)].item = true;
            addChange(new Vector2((int)tile.x, (int)tile.y));
        }
    }

    public void RemoveItem(BuildableItem item)
    {
        // Remove from room
        BuildableRoom room = getRoom(item.left, item.top);
        if (room != null)
        {
            room.items.Remove(item);
        }


        // Remove off board
        var requiredTiles = item.getItemTiles();
        foreach (Vector2 tile in requiredTiles)
        {
            tiles[(int)(tile.y * width + tile.x)].item = false;
            addChange(new Vector2((int)tile.x, (int)tile.y));
        }
    }

    // Check if we can place this item on the board
    public bool CanPlaceItem(BuildableItem item, Vector2 position)
    {
        Debug.Log("Position " + position);
        var requiredTiles = item.getItemTiles();
        foreach (Vector2 tile in requiredTiles)
        {
            Vector2 newPosition = tile;
            newPosition.x -= item.left + position.x;
            newPosition.y -= item.top + position.y;
            Debug.Log("Old Position " + tile);
            Debug.Log("New Position " + newPosition);

            if (tiles[(int)(position.y * width + position.x)].item == true )
            {
                // Already got an item here
                return false;
            }
        }
        return true;
    }

    public int maxTileTypes()
    {
        return 7;
    }


    public void RemoveFromTileMap(BuildableRoom room)
    {
        for (int i = 0; i < room.width; i++)
        {
            for (int j = 0; j < room.height; j++)
            {
                int x = room.left + i;
                int y = room.top + j;
                setTile(x, y, DTile.TYPE_GRASS);

            }
        }
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

                    Navigation.Direction dir = door.facingDirection;
                    if (dir == Navigation.Direction.North) { tiles[y * width + x].setNorthDoor(); }
                    if (dir == Navigation.Direction.South) { tiles[y * width + x].setSouthDoor(); }
                    if (dir == Navigation.Direction.West) { tiles[y * width + x].setWestDoor(); }
                    if (dir == Navigation.Direction.East) { tiles[y * width + x].setEastDoor(); }
                    //door.facingDirection = dir;
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
                addChange(new Vector2(x, y));
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
