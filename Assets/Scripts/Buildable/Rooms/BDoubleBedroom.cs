using System;
using System.Collections.Generic;
using UnityEngine;

public class BDoubleBedroom : BBedroom
{
    private Gold cost = Gold.create(50);

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
        // Make sure the AI can afford it
        if( ai.gold < purphaseCost())
        {
            return -1;
        }

        // Take their money
        ai.gold -= purphaseCost();
        data.gameLogic.addGold(purphaseCost());

        // Check the guest into every bed
        for (int i = 0; i < beds.Count; i++)
        {
            bedOwners.Add(i, ai);
        }

        // Assign them to a random bed
        return UnityEngine.Random.Range(0, beds.Count);
    }

    public override void checkout(AIBase ai)
    {
        bedOwners.Clear();
    }

    public override Gold purphaseCost()
    {
        return cost;
    }
}
