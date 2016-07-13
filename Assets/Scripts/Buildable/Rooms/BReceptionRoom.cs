﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class BReceptionRoom : BuildableRoom{

    public List<BuildableReception> frontdesks = new List<BuildableReception>();

    public List<Type> placeableItems = new List<Type>
    {
        typeof(BuildableReception)
    };



    public Dictionary<Type, int> requiredItems = new Dictionary<Type, int>
    {
        { typeof(BuildableReception),1 } // Requires 1 front desk
    };

    public override bool canBeBuilt()
    {
        return doors.Count > 0 && selectionScript.isValid();
    }

    public override List<Type> getPlaceableItems()
    {
        return placeableItems;
    }

    public override Dictionary<Type, int> getRequiredItems()
    {
        return requiredItems;
    }

    public BuildableReception getFrontDesk(int index)
    {
        return frontdesks[index];
    }

    protected override void addItem(BuildableItem item)
    {
        Debug.Log("Added item " + item);
        base.addItem(item);
        Debug.Log("Added item2 " + item);
        if (item is BuildableReception)
        {
            Debug.Log("Was Reception");
            frontdesks.Add((BuildableReception)item);
        }
    }

    public static BReceptionRoom Create(int x, int y, int width, int height, int frontDeskX, int frontDeskY, int frontDeskRotation, List<DDoor> doorPositions)
    {
        Vector3 position = new Vector3(x, 0, y);
        Vector3 size = new Vector3(width, 0, height);
        BReceptionRoom room = (BReceptionRoom)ScriptableObject.CreateInstance<BReceptionRoom>().Create(position, size);

        BuildableReception frontDesk = (BuildableReception)ScriptableObject.CreateInstance<BuildableReception>().Create(frontDeskX,frontDeskY);
        frontDesk.rotation = Quaternion.Euler(0, frontDeskRotation, 0);
        room.addItem(frontDesk);
        
        room.doors.AddRange(doorPositions);

        return room;
    }
}
