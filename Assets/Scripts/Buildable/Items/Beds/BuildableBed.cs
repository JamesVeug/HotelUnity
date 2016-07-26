using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class BuildableBed : BuildableItem, Interactable{

    public Quaternion sleepRotation = Quaternion.Euler(270, 180, 0);
    public List<Quaternion> savedInteractionRotation;
    public List<Vector3> savedInteractionPosition;
    public GameObject dirtyGameObject;
    public GameObject inUseGameObject;
    public bool isDirty = false;

    private GameData data;
    private bool soldToGuest = false;

    public bool isSoldToGuest()
    {
        return soldToGuest;
    }


    public abstract Vector2 getBedPosition(int bedSideIndex);
    public abstract int getMaxBedPositions();
    public abstract Gold purphaseCost();

    public void setRoomToDirty()
    {
        //Debug.Log("Dirty " + gameObject.name);
        dirtyGameObject.SetActive(true);
        gameObject.SetActive(false);
        inUseGameObject.SetActive(false);
        isDirty = true;
    }

    public void setRoomToInUse()
    {
        //Debug.Log("Dirty " + gameObject.name);
        inUseGameObject.SetActive(true);
        dirtyGameObject.SetActive(false);
        gameObject.SetActive(false);
        isDirty = true;
    }

    public void setRoomToClean()
    {
        //Debug.Log("Clean " + gameObject.name);
        gameObject.SetActive(true);
        dirtyGameObject.SetActive(false);
        inUseGameObject.SetActive(false);
        isDirty = false;
    }

    public void startInteracting(AIBase interactor, int index)
    {
        if(savedInteractionPosition == null) { savedInteractionPosition = new List<Vector3>();  }
        if(savedInteractionRotation== null)  { savedInteractionRotation = new List<Quaternion>();  }
        while (savedInteractionPosition.Count <= index)
        {
            savedInteractionPosition.Add(Vector3.zero);
            savedInteractionRotation.Add(Quaternion.identity);
        }

        savedInteractionPosition[index] = interactor.transform.position;
        savedInteractionRotation[index] = interactor.transform.localRotation;

        if (data == null) data = FindObjectOfType<GameData>();
        Vector2 pos = getBedPosition(index);
        pos.x += data.graphicsMap.tileSize / 2;
        pos.y -= data.graphicsMap.tileSize - 0.15f;

        interactor.transform.position = new Vector3(pos.x,0.65f,pos.y);
        interactor.transform.localRotation = sleepRotation;
    }

    public void stopInteracting(AIBase interactor, int index)
    {
        interactor.transform.position = savedInteractionPosition[index];
        interactor.transform.localRotation = savedInteractionRotation[index];
    }
}
