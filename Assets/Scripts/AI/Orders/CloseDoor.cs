using UnityEngine;
using System.Collections;
using System;

public class CloseDoor : Order {

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        DDoor door = (DDoor)ai.currentInteraction;
        bool isOpen = door.isOpen();
        ai.currentInteraction.stopInteracting(ai, ai.id);
        ai.currentInteraction = null;

        ai.State = ai.STATE_WALK;

        if (!isOpen)
        {
            if (ai is GuestAI) ((GuestAI)ai).Update();
            if (ai is StaffAI) ((StaffAI)ai).Update();
        }

        return RETURN_TYPE.COMPLETED;
    }
}
