using UnityEngine;
using System;
using System.Collections.Generic;

public class DDoor : DDataObject, Interactable
{
    public GameObject openObject;
    public GameObject closeObject;
    private bool state_open = false;
    public List<AIBase> peopleByDoor = new List<AIBase>();

    public bool isOpen()
    {
        return state_open;
    }

    public void open()
    {
        closeObject.SetActive(false);
        openObject.SetActive(true);
        state_open = true;
    }

    public void close()
    {
        closeObject.SetActive(true);
        openObject.SetActive(false);
        state_open = false;
    }


    public static DDoor create(float x, float y, Navigation.Direction dir)
    {
        DDoor door = ScriptableObject.CreateInstance<DDoor>();
        door.position = new Vector2(x, y);
        door.facingDirection = dir;

        return door;
    }

    public static DDoor create(Vector2 position, Navigation.Direction dir)
    {
        DDoor door = ScriptableObject.CreateInstance<DDoor>();
        door.position = new Vector2(position.x, position.y);
        door.facingDirection = dir;

        return door;
    }

    public void startInteracting(AIBase interactor, int index)
    {
        if( peopleByDoor.Count == 0)
        {
            open();
        }
        peopleByDoor.Add(interactor);
    }

    public void stopInteracting(AIBase interactor, int index)
    {
        if (peopleByDoor.Count == 1)
        {
            close();
        }
        peopleByDoor.Remove(interactor);
    }
}
