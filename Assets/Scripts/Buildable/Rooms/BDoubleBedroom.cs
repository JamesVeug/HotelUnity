using System;
using System.Collections.Generic;
using UnityEngine;

public class BDoubleBedroom : BBedroom{


    public List<Type> placeableItems = new List<Type>
    {
        typeof(BuildableMinibar),
        typeof(ISingleBed),
        typeof(IDoubleBed)
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
            bedOwners.Add(i, ai);
        }

        return UnityEngine.Random.Range(0, beds.Count);
    }

    public override void checkout(AIBase ai)
    {
        bedOwners.Clear();
    }
}
