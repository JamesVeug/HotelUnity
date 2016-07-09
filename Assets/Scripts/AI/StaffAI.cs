using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class StaffAI : AIBase
{


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
            if (order.executeOrder(this, nav))
            {
                // Finished the order.
                // Get rid of it
                Order completed = orders.Pop();
                if (completed is GoHome)
                {
                    Destroy(gameObject);
                }
                else if (completed is Sleep)
                {
                    BuildableBed bed = getOwnedBed();
                    bed.setRoomToDirty();
                }
            }
        }
    }


}
