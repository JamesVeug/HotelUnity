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
}
