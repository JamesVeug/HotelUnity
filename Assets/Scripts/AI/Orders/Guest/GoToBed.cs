using UnityEngine;
using System.Collections;

public class GoToBed : Order
{
	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        // Don't own a room/bed
        if( ai.getOwnedRoom() == null)
        {
            ai.addOrder(ScriptableObject.CreateInstance<BuyBed>());
			return RETURN_TYPE.PROBLEM;
        }

        // Are we in the owned room
        if( !ai.getCurrentRoom().Equals(ai.getOwnedRoom()) )
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToRoom>());
			return RETURN_TYPE.PROBLEM;
        }

        // Walk to bed
        Vector3 bedPosition = nav.getBedPosition(ai);
        Vector2 bedTile = new Vector2(bedPosition.x, bedPosition.z);
        if( (!ai.isByTile(bedTile)) )
        {
            ai.walkToItem(bedPosition);
			return RETURN_TYPE.PROBLEM;
        }

        // We are at beds position
        ai.currentInteraction = ai.getOwnedBed();
		return RETURN_TYPE.COMPLETED;
    }

}
