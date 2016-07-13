using UnityEngine;
using System.Collections;
using System;

public class OpenDoor : Order {

    private DDoor door;

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        ai.currentInteraction = door;
        ai.currentInteraction.startInteracting(ai, ai.id);

        ai.State = ai.STATE_WALK;

        return RETURN_TYPE.COMPLETED;
    }

    public OpenDoor setDoor(DDoor door)
    {
        this.door = door;
        return this;
    }
}
