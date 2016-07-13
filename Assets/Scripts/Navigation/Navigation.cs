
using System;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : ScriptableObject{
    public enum Direction { North, East, South, West };
    public List<Vector2> AISpawnLocations = new List<Vector2>();
    private GameData data;  

    public void initialize()
    {
        data = GameObject.FindObjectOfType<GameData>();
    }

    public BReceptionRoom getNearestReceptionRoom(AIBase ai)
    {
        List<BReceptionRoom> rooms = data.dTileMap.getReceptionRooms();

        float tileSize = data.graphicsMap.tileSize;
        Vector2 aiPosition = new Vector2(Mathf.Floor(ai.transform.position.x/ tileSize), Mathf.Floor(ai.transform.position.z/ tileSize));
        float minDistance = float.MaxValue;
        BReceptionRoom closestRoom = null;
        foreach(BReceptionRoom r in rooms)
        {
            foreach (DDoor d in r.doors)
            {
                float distance = (aiPosition - d.position).magnitude;
                if( distance < minDistance)
                {
                    closestRoom = r;
                    minDistance = distance;
                }
            }
        }

        // Return closest room according to the doors
        return closestRoom;
    }

    public BHouseKeepingRoom getNearestHouseKeepingRoom(AIBase ai)
    {
        List<BHouseKeepingRoom> rooms = data.dTileMap.getHouseKeepingRooms();

        float tileSize = data.graphicsMap.tileSize;
        Vector2 aiPosition = new Vector2(Mathf.Floor(ai.transform.position.x / tileSize), Mathf.Floor(ai.transform.position.z / tileSize));
        float minDistance = float.MaxValue;
        BHouseKeepingRoom closestRoom = null;
        foreach (BHouseKeepingRoom r in rooms)
        {
            foreach (DDoor d in r.doors)
            {
                float distance = (aiPosition - d.position).magnitude;
                if (distance < minDistance)
                {
                    closestRoom = r;
                    minDistance = distance;
                }
            }
        }

        // Return closest room according to the doors
        return closestRoom;
    }

    public Vector3 getClosestDoorPosition(AIBase ai, BuildableRoom room)
    {
        float tileSize = data.graphicsMap.tileSize;
        Vector2 aiPosition = new Vector2(Mathf.Floor(ai.transform.position.x / tileSize), Mathf.Floor(ai.transform.position.z / tileSize));

        float minDistance = float.MaxValue;
        Vector3 position = Vector3.zero;
        foreach (DDoor d in room.doors)
        {
            float distance = (aiPosition - d.position).magnitude;
            if (distance < minDistance)
            {
                position = new Vector3(d.position.x,0,d.position.y);
                minDistance = distance;
            }
        }

        //Debug.Log("Closest door " + minDistance + " " + position);
        return position;
    }

    public Vector3 getBedPosition(AIBase ai)
    {
        BuildableBed bed = ai.getOwnedRoom().getBed(ai.getBedIndex());
        Vector2 bedPosition = bed.getBedPosition(ai.getBedSideIndex());

        return new Vector3(bedPosition.x, 0, bedPosition.y);
    }

    public BuildableRoom getCurrentRoom(AIBase ai)
    {
        float tileSize = data.graphicsMap.tileSize;
        Vector2 aiPosition = new Vector2(Mathf.Floor(ai.transform.position.x / tileSize), Mathf.Floor(ai.transform.position.z / tileSize));
        return data.dTileMap.getRoom((int)aiPosition.x, (int)aiPosition.y);
    }

    public List<Vector3> getPathToItem(Vector3 currentPosition, Vector3 itemPosition)
    {
        // Get closest point to item according to where they currently are
        List<Vector3> exceptions = new List<Vector3>();
        exceptions.Add(itemPosition);

        return getPath(currentPosition, itemPosition, exceptions);
    }

    public List<Vector3> getPath(Vector3 start, Vector3 end, List<Vector3> exceptions)
    {
        List<Vector2> tileExceptions = new List<Vector2>();
        if( exceptions != null)
        {
            for(int i = 0; i < exceptions.Count; i++)
            {
                Vector2 pos = new Vector2((int)(exceptions[0].x / data.graphicsMap.tileSize), (int)(exceptions[0].z / data.graphicsMap.tileSize));
                tileExceptions.Add(pos);
            }
        }

        Vector2 endTile = new Vector2((int)(start.x/data.graphicsMap.tileSize),(int)(start.z / data.graphicsMap.tileSize));
        Vector2 startTile = new Vector2((int)(end.x / data.graphicsMap.tileSize), (int)(end.z / data.graphicsMap.tileSize));

        HashSet<Vector2> visited = new HashSet<Vector2>();
        List<AStarNode> fringe = new List<AStarNode>();
        addNode(fringe, null, startTile, endTile);

        //Debug.Log("Beginning AStar Positions " + start + " -> " + end);
        //Debug.Log("Beginning AStar Tiles     " + startTile + " -> " + endTile);

        AStarNode closest = null;

        bool foundPath = false;
        AStarNode star = null;
        while (fringe.Count > 0)
        {
            star = fringe[0]; fringe.RemoveAt(0);
            Vector2 node = star.current;

            //Debug.Log("node " + node);

            //if ( star.last != null )
            //Debug.DrawLine(new Vector3(node.x, 1, node.y), new Vector3(star.last.current.x, 1, star.last.current.y), Color.red, 5);

            if( closest == null || closest.heuristic > star.heuristic)
            {
                closest = star;
            }

            // Found target
            if (star.heuristic <= 0)
            {
                foundPath = true;
                break;
            }
            
            if( star.last != null )
            {
                //Debug.Log("heuristic " + heuristic + " " + star.last.current + "  -> " + node);
            }

            // Don't visit if we already have
            if (visited.Contains(node))
            {
                continue;
            }
            visited.Add(node);


            // Visit every tile around us
            // Horizontal
            //Debug.Log("Right");
            Vector2 rightPosition = new Vector2(node.x + 1, node.y);
            if (node.x + 1 < data.dTileMap.width && canBeWalkedOn3(rightPosition, node, Direction.East, tileExceptions))
            {
                addNode(fringe, star, rightPosition, endTile);
            }
            //else { Debug.DrawLine(new Vector3(node.x, 0.5f, node.y), new Vector3(rightPosition.x, 0.5f, rightPosition.y), Color.red, 1); }


            //Debug.Log("Left");
            Vector2 leftPosition = new Vector2(node.x - 1, node.y);
            if (node.x > 0 && (canBeWalkedOn3(leftPosition, node, Direction.West, tileExceptions)) )
            {
                addNode(fringe, star, leftPosition, endTile);
            }
            //else { Debug.DrawLine(new Vector3(node.x, 0.5f, node.y), new Vector3(leftPosition.x, 0.5f, leftPosition.y), Color.red, 1); }


            //Vertical
            //Debug.Log("Up");
            Vector2 upPosition = new Vector2(node.x, node.y+1);
            if (node.y + 1 < data.dTileMap.height && (canBeWalkedOn3(upPosition, node, Direction.North, tileExceptions)))
            {
                addNode(fringe, star, upPosition, endTile);
            }
            //else { Debug.DrawLine(new Vector3(node.x, 0.5f, node.y), new Vector3(upPosition.x, 0.5f, upPosition.y), Color.red, 1); }


            //Debug.Log("Down");
            Vector2 downPosition = new Vector2(node.x, node.y-1);
            if (node.y > 0 && (canBeWalkedOn3(downPosition, node, Direction.South, tileExceptions)))
            {
                addNode(fringe, star, downPosition, endTile);
            }
            //else { Debug.DrawLine(new Vector3(node.x, 0.5f, node.y), new Vector3(downPosition.x, 0.5f, downPosition.y), Color.red, 1); }
        }


        List<Vector3> nodes = new List<Vector3>();

        // Couldn't find the path
        if (!foundPath)
        {
            Debug.Log("Couldn't find path from " + startTile + " to " + endTile);
            Debug.Log("Closest path " + " with a heuristic of " + closest.heuristic + " " + closest.ToString());
            Debug.DrawLine(start+new Vector3(0,1,0), end + new Vector3(0, 1, 0), Color.red, 10);
            return nodes;
        }
        else
        {

            //Debug.Log("Final path: " + endTile + " === " + star.ToString());
        }


        Vector2 offset = new Vector2(data.graphicsMap.tileSize / 2, data.graphicsMap.tileSize / 2);
        AStarNode it = star;
        while (it != null)
        {
            if( it.last != null ){
                Debug.DrawLine(new Vector3(it.current.x, 0.5f, it.current.y), new Vector3(it.last.current.x, 0.5f, it.last.current.y), Color.green, 1);
            }

            if (!tileExceptions.Contains(it.current))
            {
                Vector2 offsetedPosition = it.current + offset;
                Vector3 position = new Vector3(offsetedPosition.x, start.y, offsetedPosition.y);
                nodes.Add(position);
            }
            else
            {
                //Debug.Log("Found exception that isn't added to Path: " + it.current);
            }
            it = it.last;
        }
        
        return nodes;
    }

    public bool canBeWalkedOn3(Vector2 to, Vector2 from, Direction dir, List<Vector2> exceptions)
    {
        DTileMap map = data.dTileMap;
        DTile toTile = map.getTile((int)to.x, (int)to.y);
        DTile fromTile = map.getTile((int)from.x, (int)from.y);

        //Debug.Log("Tiles + " + from + "->" + to + " " + dir + " " + toTile.northHasWall());

        // Types of tiles
        if (toTile.hasItem() && !exceptions.Contains(to)) return false;
        if (toTile.isGrass()) return false;

        // Walls
        if (dir == Direction.North && (toTile.southHasWall() || fromTile.northHasWall())) return false;
        if (dir == Direction.East && (toTile.westHasWall() || fromTile.eastHasWall())) return false;
        if (dir == Direction.West && (toTile.eastHasWall() || fromTile.westHasWall())) return false;
        if (dir == Direction.South && (toTile.northHasWall() || fromTile.southHasWall())) return false;

        // Windows
        if (dir == Direction.North && (toTile.southHasWindow() || fromTile.northHasWindow())) return false;
        if (dir == Direction.East && (toTile.westHasWindow() || fromTile.eastHasWindow())) return false;
        if (dir == Direction.West && (toTile.eastHasWindow() || fromTile.westHasWindow())) return false;
        if (dir == Direction.South && (toTile.northHasWindow() || fromTile.southHasWindow())) return false;

        toTile.drawDebugTile(to, 1, Color.green, 2);
        return true;
    }

    public bool canBeWalkedOn2(Vector2 to, Vector2 from)
    {
        int xTo = (int)to.x;
        int yTo = (int)to.y;
        int xFrom = (int)from.x;
        int yFrom = (int)from.y;

        DTileMap map = data.dTileMap;
        BuildableRoom toRoom = map.getRoom(xTo, yTo);
        BuildableRoom fromRoom = map.getRoom(xFrom, yFrom);
        Debug.Log("Info " + fromRoom + " -> " + toRoom + " as positions + " + from + " -> " + to);

        // Outside
        if (toRoom == null && fromRoom == null)
        {
            Debug.Log("A");
            if( !map.isWalkWay(xTo,yTo))
            {
                return false;
            }
        }

        // Moving between rooms
        if( toRoom == null && fromRoom != null)
        {
            Debug.Log("B");
            //Debug.Log("Moving OUT OF a room " + fromRoom.position + " as positions + " + to + " -> " + from);

            // Walking from outside to inside
            // Can only do this via door
            if ( !map.hasDoor(xFrom, yFrom) ){
                return false;
            }
        }
        else if (toRoom != null && fromRoom == null)
        {
            Debug.Log("C");
            //Debug.Log("Moving INTO a room " + toRoom.position + " as positions + " + to + " -> " + from);

            // Walking from outside to inside
            // Can only do this via door
            if (!map.hasDoor(xTo, yTo))
            {
                return false;
            }
        }

        // Moving around in a room
        if (toRoom != null && fromRoom != null)
        {
            //Debug.Log("Moving around in a rooms " + toRoom.position + " -> " + fromRoom.position + " as positions + " + to + " -> " + from);
            if (!toRoom.Equals(fromRoom))
            {
                Debug.Log("Different Rooms");
                // Moving between rooms only if they are not walls
                //if (!map.isDoor(xTo, yTo) || !map.isDoor(xFrom, yFrom))
                //{
                //}

                // No... none of this!
                return false;
            }
            else
            {
                Debug.Log("Same Room");

                if (map.hasItem(xTo, yTo))
                {
                    return false;
                }
            }
        }


        /*if( toRoom != null && fromRoom != null && toRoom != fromRoom)
        {
            Debug.Log("Different rooms already! ");
            Debug.LogError("Info " + toRoom.position + " -> " + fromRoom.position + " as positions + " + to + " -> " + from);

        }*/
        return true;
    }

    public bool canBeWalkedOn(float xTo, float yTo, float xFrom, float yFrom)
    {
        DTileMap map = data.dTileMap;
        if (map.isWalkWay((int)xTo, (int)yTo)) {
            if (map.isWalkWay((int)xFrom, (int)yFrom))
            {
                // From Walkway to Walkway
                return true;
            }
            else if (map.hasDoor((int)xFrom, (int)yFrom))
            {
                // From Door to Walkway
                return true;
            }
        }
        if (map.isRoom((int)xTo, (int)yTo)) return true;
        if (map.hasDoor((int)xTo, (int)yTo)) return true;
        if (map.hasWall((int)xTo, (int)yTo))
        {
            if (map.isRoom((int)xFrom, (int)yFrom)) {
                // From Room to Wall
                return true;
            }
            else if (map.hasWall((int)xFrom, (int)yFrom)){
                if (  map.getRoom((int)xTo, (int)yTo) == map.getRoom((int)xFrom, (int)yFrom)) {
                    // From Wall to Wall in the same room
                    return true;
                }
            }
        }
        if (map.hasItem((int)xTo, (int)yTo))
        {
            Debug.Log("Position " + xTo + "," + yTo + " " + map.getTileType((int)xTo, (int)yTo) + " - " + xFrom + "," + yFrom + " " + map.getTileType((int)xFrom, (int)yFrom));
            if (map.hasWall((int)xFrom, (int)yFrom))
            {
                Debug.Log("Rooms " + map.getRoom((int)xTo, (int)yTo).position + " -> " + map.getRoom((int)xFrom, (int)yFrom).position);
                if (map.getRoom((int)xTo, (int)yTo) == map.getRoom((int)xFrom, (int)yFrom))
                {
                    Debug.Log("Same Room");
                    // From Wall to Wall in the same room
                    return true;
                }
            }
        }

        // Can not walk on this path
        return false;
    }

    private void addNode(List<AStarNode> list, AStarNode last, Vector2 toAdd, Vector2 target)
    {
        // Distance from this node to the master shard
        float heuristic = (toAdd-target).magnitude;

        float distanceToNextPoint = last == null ? 0 : (last.current-toAdd).magnitude;
        float distance = last != null ? last.distance + distanceToNextPoint : 0;
        AStarNode path = new AStarNode(distance, heuristic, last, toAdd);

        for (int i = 0; i < list.Count; i++)
        {


            if (list[i].compareTo(path) > 0)
            {
                list.Insert(i, path);
                return;
            }
        }

        // Didn't add into the list. So add to the end
        list.Add(path);
    }

    public Vector2 getRandomSpawnPosition()
    {
        int index = UnityEngine.Random.Range(0, AISpawnLocations.Count);
        return AISpawnLocations[index];
    }
}
