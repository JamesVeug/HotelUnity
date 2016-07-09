using UnityEngine;
using System.Collections;
using System;

public class GetInBed : Order
{
    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        // Buy a bed
        if( ai.getOwnedBed() == null)
        {
            ai.addOrder(ScriptableObject.CreateInstance<BuyBed>());
            return false;
        }

        // Check we are interacting with the bed
        if ( (System.Object)ai.currentInteraction != ai.getOwnedBed() )
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToBed>());
            return false;
        }

        // Get in bed
        ai.currentInteraction.startInteracting(ai, ai.getBedSideIndex());

        // We are in bed
        return true;
    }
}
