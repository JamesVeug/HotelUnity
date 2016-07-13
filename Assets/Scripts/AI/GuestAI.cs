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

    // Update is called once per frame
    void Update()
    {
        if (State == STATE_WALK)
        {
            moveTowardsTarget();
        }
        else if (State == STATE_IDLE)
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
			Order.RETURN_TYPE status = order.executeOrder (this, nav);

			if (status == Order.RETURN_TYPE.COMPLETED) {
				orders.Pop ();
				
				// Finished the order successfully.
				// Get rid of it
				if (order is GoHome) {
					Destroy (gameObject);
				} else if (order is GetInBed) {
					BuildableBed bed = getOwnedBed ();
					bed.setRoomToInUse ();
				} else if (order is Sleep) {
					BuildableBed bed = getOwnedBed ();
					bed.setRoomToDirty ();
				}
			} else if (status == Order.RETURN_TYPE.FAILED) {
				if (order is BuyBed) {
					
					// There are no beds available
					// Get angry and go home
					orders.Clear();
					orders.Push (ScriptableObject.CreateInstance<GoHome>());
				}

			}
        }
    }

    
}
