using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class AIBase : MonoBehaviour
{
    public static int NEXT_ID = 0;
    public int id = 0;

    public int STATE_IDLE = 0;
    public int STATE_WALK = 1;
    public int STATE_SLEEPING = 2;

    public float tileMinDistance = 0.1f;
    public float walkSpeed = 0.1f;

    public int State = 0;

    protected List<Vector3> path = new List<Vector3>();
    protected int pathIndex = 0;

    public Vector3 targetPosition { get; set; }

    protected Navigation nav;
    protected GameData data;

    protected Stack<Order> orders = new Stack<Order>();

    protected int bedIndex = 0;
    protected int bedSideIndex = 0;

    protected BBedroom ownedRoom = null;
    protected BuildableRoom currentRoom = null;
    public Interactable currentInteraction;

    // Properties
    public float property_sleep = 0;
    public float property_anger = 0;


    public void walkToItem(Vector3 pos)
    {
        //Debug.Log("Door position " + pos);
        targetPosition = pos;
        State = STATE_WALK;
        path = nav.getPathToItem(transform.position, targetPosition);
        pathIndex = 0;
    }

    public void walkToPosition(Vector3 pos)
    {
        //Debug.Log("Door position " + pos);
        targetPosition = pos;
        State = STATE_WALK;
        path = nav.getPath(transform.position, targetPosition, null);
        pathIndex = 0;
    }

    protected void sleep()
    {
        if (property_sleep >= 1)
        {
            // Wake up
            State = STATE_IDLE;
        }
        else
        {
            // Make Z's
            property_sleep += Time.deltaTime;
            Debug.Log("Sleep");
        }
    }

    public int getBedIndex()
    {
        return bedIndex;
    }

    public int getBedSideIndex()
    {
        return bedSideIndex;
    }

    public BuildableRoom getCurrentRoom()
    {
        return currentRoom;
    }

    public BBedroom getOwnedRoom()
    {
        return ownedRoom;
    }

    public void addOrder(Order order)
    {
        orders.Push(order);
    }

    public bool buyRoom()
    {
        BBedroom room = data.gameLogic.getAvailableRoom();

        if (room == null)
        {
            // No rooms available
            return false;
        }

        // Check us in
        bedIndex = room.checkin(this);
        if( bedIndex == -1)
        {
            // Couldn't check in
            return false;
        }

        ownedRoom = room;
        bedSideIndex = 0;

        // Checked in
        return true;
    }

    public void checkOut()
    {
        // Check us out of the room
        ownedRoom.checkout(this);

        // Forget the room
        ownedRoom = null;
        bedIndex = 0;
        bedSideIndex = 0;
    }

    public bool isAtTile(Vector2 position)
    {
        Vector3 worldTilePosition = new Vector3(position.x, 0, position.y);
        Vector3 worldPosition = new Vector3(transform.position.x - data.graphicsMap.tileSize / 2, 0, transform.position.z - data.graphicsMap.tileSize / 2);
        return (worldTilePosition - worldPosition).magnitude <= tileMinDistance;
    }



    public bool isByTile(Vector2 position)
    {
        Vector3 worldTilePosition = new Vector3(position.x, 0, position.y);
        Vector3 worldPosition = new Vector3(transform.position.x - data.graphicsMap.tileSize / 2, 0, transform.position.z - data.graphicsMap.tileSize / 2);
        return (worldTilePosition - worldPosition).magnitude <= 2;
    }

    public BuildableBed getOwnedBed()
    {
        BBedroom room = getOwnedRoom();
        if (room == null)
        {
            return null;
        }
        
        return room.getBed(getBedIndex());
    }

    public Order getCurrentOrder()
    {
        return orders.Count > 0 ? orders.Peek() : null;
    }

    protected void moveTowardsTarget()
    {
        if (path == null)
        {
            Debug.LogError("Path is null - target = " + targetPosition);
        }
        else if (path.Count == 0)
        {
            Debug.DrawLine(transform.position, targetPosition, Color.red, 1);
            Debug.LogError("No points in path - target = " + targetPosition);
        }
        else if (pathIndex >= path.Count)
        {
            Debug.LogError("Attempting to walk past path " + pathIndex + " >= " + path.Capacity + " - target = " + targetPosition);
        }

        // Check next path
        //Debug.Log("Index " + pathIndex + " " + path[pathIndex]);
        //Debug.Log("Distance " + distance);

        //Debug.Log(path[pathIndex] + " " +  transform.position);
        float distance = (path[pathIndex] - transform.position).magnitude;
        if (distance < tileMinDistance)
        {
            // Finished path
            // Changed room as we are on a new tile
            currentRoom = nav.getCurrentRoom(this);

            pathIndex++;


            if (pathIndex >= path.Count)
            {
                targetPosition = Vector3.zero;
                State = STATE_IDLE;
                return;
            }

            // Next point is valid

            //Debug.Log("New Point " + path[pathIndex]);

            if ((path[pathIndex]-new Vector3(7.5f, 0.0f, 22.5f)).magnitude < 0.1f)
            {
                //Debug.Log("========= North Door");
            }

            if ( blockedByDoor(path[pathIndex-1], path[pathIndex]) )
            {
                //Debug.Log("Blocked");

                // We can't move as we need to open the door
                DDoor door = data.dTileMap.getDoor((int)path[pathIndex].x, (int)path[pathIndex].z);
                if (door == null)
                {
                    door = data.dTileMap.getDoor((int)path[pathIndex - 1].x, (int)path[pathIndex - 1].z);
                }

                // Check we still don't have a door
                if (door == null)
                {
                    //Debug.Log("Can't find door at " + path[pathIndex] + " or " + path[pathIndex-1]);
                }
                else {
                    //Debug.Log("Door exists");
                    // Open the door
                    addOrder(ScriptableObject.CreateInstance<OpenDoor>().setDoor(door));
                    State = STATE_IDLE;
                    return;
                }
            }
            else if (currentInteraction is DDoor)
            {
                // Close the door as we are on a new tile
                addOrder(ScriptableObject.CreateInstance<CloseDoor>());
                State = STATE_IDLE;
                return;
            }

        }

        // Moving to next point
        Vector3 newPosition = Vector3.MoveTowards(transform.position, path[pathIndex], Time.deltaTime * walkSpeed);
        transform.LookAt(newPosition);

        transform.position = newPosition;
    }

    private bool blockedByDoor(Vector3 current, Vector3 next)
    {
        // While walking to the target, we need to check that the next tile doesn't contain a door
        return nav.blockedByDoor(current, next);
    }
}
