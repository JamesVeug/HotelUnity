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

    public int bedCount()
    {
        return beds.Count;
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
        foreach (BuildableBed b in beds)
        {
            if (!b.isDirty && !b.isSoldToGuest())
            {
                return true;
            }
        }

        return false;
    }

    public bool hasDirtyBeds()
    {
        foreach(BuildableBed b in beds)
        {
            if( b.isDirty)
            {
                return true;
            }
        }

        return false;
    }

    public abstract int checkin(AIBase ai);
    public abstract void checkout(AIBase ai);
}
