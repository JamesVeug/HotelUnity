using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class StaffAI : AIBase
{
    public BBedroom dirtyRoom = null;
    public BuildableBed dirtyBed = null; // Current bed we are cleaning

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        data = GameObject.FindObjectOfType<GameData>();
        nav = data.navigation;

        id = NEXT_ID++;
        name = "BasicAI(" + id + ")";

    }

    // Update is called once per frame
    void Update()
    {



        if (State == STATE_IDLE)
        {
            idle();
        }
        else if (State == STATE_WALK)
        {
            moveTowardsTarget();
        }
    }

    private void idle()
    {
        if (orders.Count == 0)
        {
            // Go to sleep
            orders.Push(ScriptableObject.CreateInstance<CheckForDirtyRooms>());
        }
        else {

            // Process the next oder
            Order order = orders.Peek();
			if (order.executeOrder(this, nav) == Order.RETURN_TYPE.COMPLETED )
            {
                orders.Pop();

                if( order is CheckForDirtyRooms)
                {
                    orders.Push(ScriptableObject.CreateInstance<CleanRoom>());
                }
                else if (order is CleanRoom)
                {
                    orders.Push(ScriptableObject.CreateInstance<CheckForDirtyRooms>());
                }
            }
        }
    }

    public bool assignDirtyRoom()
    {
        // Check Current Room
        if( currentRoom is BBedroom)
        {

            return false;
        }

        BBedroom room = data.gameLogic.getDirtyRoom();
        if( room == null)
        {
            return false;
        }


        dirtyRoom = room;
        return true;
    }
}
