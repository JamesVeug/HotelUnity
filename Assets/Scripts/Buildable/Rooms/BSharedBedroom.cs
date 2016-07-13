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
            if(!beds[i].isDirty && !bedOwners.ContainsKey(i))
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

    public static BSharedBedroom Create(int x, int y, int width, int height, List<DDoor> doorPositions, List<BuildableBed> beds)
    {
        Vector3 position = new Vector3(x, 0, y);
        Vector3 size = new Vector3(width, 0, height);
        BSharedBedroom room = (BSharedBedroom)ScriptableObject.CreateInstance<BSharedBedroom>().Create(position, size);

        foreach(BuildableBed bed in beds)
        {
            room.addItem(bed);
        }

        room.doors.AddRange(doorPositions);

        return room;
    }
}
