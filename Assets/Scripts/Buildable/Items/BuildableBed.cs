using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class BuildableBed : BuildableItem{

    public GameObject dirtyGameObject;
    public bool isDirty = false;


    private bool soldToGuest = false;

    public bool isSoldToGuest()
    {
        return soldToGuest;
    }

    public abstract Vector2 getBedPosition(int bedSideIndex);
    public abstract int getMaxBedPositions();

    public void setRoomToDirty()
    {
        Debug.Log("Dirty " + gameObject.name);
        dirtyGameObject.SetActive(true);
        gameObject.SetActive(false);
        isDirty = true;
    }

    public void setRoomToClean()
    {
        Debug.Log("Clean " + gameObject.name);
        dirtyGameObject.SetActive(false);
        gameObject.SetActive(true);
        isDirty = false;
    }
}
