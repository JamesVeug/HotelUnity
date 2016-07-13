using UnityEngine;
using System.Collections;
using System;

public class GetInBed : Order
{
	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        // Buy a bed
        if( ai.getOwnedBed() == null)
        {
            ai.addOrder(ScriptableObject.CreateInstance<BuyBed>());
			return RETURN_TYPE.PROBLEM;
        }

        // Check we are interacting with the bed
        if ( (System.Object)ai.currentInteraction != ai.getOwnedBed() )
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToBed>());
			return RETURN_TYPE.PROBLEM;
        }

        // Get in bed
        ai.currentInteraction.startInteracting(ai, ai.getBedSideIndex());

        // We are in bed
		return RETURN_TYPE.COMPLETED;
    }
}
