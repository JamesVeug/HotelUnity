using UnityEngine;
using System.Collections;
using System;

public class GetOutOfBed : Order
{
    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        if (ai.currentInteraction != null)
        {
            // Get out of bed if we are in it
            ai.currentInteraction.stopInteracting(ai, ai.getBedSideIndex());
            ai.currentInteraction = null;
        }

        // Wake up
        return true;
    }
}
