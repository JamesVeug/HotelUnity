using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BBedroom : BuildableRoom{
    
    protected List<BuildableBed> beds = new List<BuildableBed>();
    protected Dictionary<int, AIBase> bedOwners = new Dictionary<int, AIBase>();


    public Dictionary<Type, int> requiredItems = new Dictionary<Type, int>
    {
        { typeof(BuildableBed),1 },
    };

    public override bool canBeBuilt()
    {
        return doors.Count > 0 && beds.Count > 0 && selectionScript.isValid();
    }

    public BuildableBed getBed(int v)
    {
        return beds[v];
    }

    protected override void addItem(BuildableItem item)
    {
        base.addItem(item);
        if (item is BuildableBed)
        {
            beds.Add((BuildableBed)item);
        }

    }

    public override Dictionary<Type, int> getRequiredItems()
    {
        return requiredItems;
    }

    public bool hasBedsAvailable()
    {
        //Debug.Log("Beds " + bedOwners.Count + " , " + beds.Count);
        return bedOwners.Count < beds.Count;
    }

    public abstract int checkin(AIBase ai);
    public abstract void checkout(AIBase ai);
}
