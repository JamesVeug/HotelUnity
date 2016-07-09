using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GuestAI : AIBase
{


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start () {
        data = GameObject.FindObjectOfType<GameData>();
        nav = data.navigation;

        // Randomize color
        GameObject mesh = transform.FindChild("Body").gameObject;

        Renderer rend = mesh.GetComponent<Renderer>();
        rend.material.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        id = NEXT_ID++;

        name = "BasicAI(" + id + ")";


        // Random properties
        property_sleep = UnityEngine.Random.Range(0, 0.5f);
    }

    void FixedUpdate()
    {
        if (State == STATE_WALK)
        {
            moveTowardsTarget();
        }
    }

    // Update is called once per frame
    void Update () {

        

        if( State == STATE_IDLE)
        {
            idle();
        }
	}

    private void idle()
    {
        if (orders.Count == 0)
        {
            // Buy a bed
            // Go to sleep
            // Go home
            orders.Push(ScriptableObject.CreateInstance<GoHome>());
            orders.Push(ScriptableObject.CreateInstance<GetOutOfBed>());
            orders.Push(ScriptableObject.CreateInstance<Sleep>());
            orders.Push(ScriptableObject.CreateInstance<GetInBed>());
        }
        else {

            // Process the next oder
            Order order = orders.Peek();
            if (order.executeOrder(this, nav))
            {
                // Finished the order.
                // Get rid of it
                Order completed = orders.Pop();
                if(completed is GoHome)
                {
                    Destroy(gameObject);
                }
                else if (completed is GetInBed)
                {
                    BuildableBed bed = getOwnedBed();
                    bed.setRoomToInUse();
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
