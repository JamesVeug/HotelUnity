using System;
using System.Collections.Generic;
using UnityEngine;

public class BHouseKeepingRoom : BuildableRoom{

    public List<Type> placeableItems = new List<Type>
    {
        typeof(BuildableChair),
        typeof(BuildablePainting)
    };

    public override bool canBeBuilt()
    {
        return selectionScript.isValid();
    }

    public override List<Type> getPlaceableItems()
    {
        return placeableItems;
    }

    public override Dictionary<Type, int> getRequiredItems()
    {
        return new Dictionary<Type, int>();
    }

    public static BHouseKeepingRoom Create(int x, int y, int width, int height, List<DDoor> doors)
    {
        Vector3 position = new Vector3(x, 0, y);
        Vector3 size = new Vector3(width, 0, height);
        BHouseKeepingRoom room = (BHouseKeepingRoom)ScriptableObject.CreateInstance<BHouseKeepingRoom>().Create(position, size);

        room.doors.AddRange(doors);

        return room;
    }
}
