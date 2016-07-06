using System;
using System.Collections.Generic;
using UnityEngine;

public class BSharedBedroom : BBedroom{


    public List<Type> placeableItems = new List<Type>
    {
        typeof(BuildableMinibar),
        typeof(ISingleBed),
        typeof(IDoubleBed),
    };

    public override bool canBeBuilt()
    {
        return doors.Count > 0 && selectionScript.isValid();
    }

    public override List<Type> getPlaceableItems()
    {
        return placeableItems;
    }

    public override int checkin(AIBase ai)
    {
        for(int i = 0; i < beds.Count; i++)
        {
            if(!bedOwners.ContainsKey(i))
            {
                bedOwners.Add(i,ai);
                return i;
            }
        }

        return -1;
    }

    public override void checkout(AIBase ai)
    {
        for(int i = 0; i < beds.Count; i++)
        {
            if (bedOwners.ContainsKey(i) && bedOwners[i].Equals(ai))
            {
                bedOwners.Remove(i);
                return;
            }
        }
        Debug.Log("Could not remove ai from room!");
    }
}
