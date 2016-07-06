using UnityEngine;
using System.Collections;

public class GoToBed : Order
{
    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        // Don't own a room/bed
        if( ai.getOwnedRoom() == null)
        {
            ai.addOrder(ScriptableObject.CreateInstance<BuyBed>());
            return false;
        }

        // Are we in the owned room
        if( !ai.getCurrentRoom().Equals(ai.getOwnedRoom()) )
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToRoom>());
            return false;
        }

        // Walk to bed
        Vector3 bedPosition = nav.getBedPosition(ai);
        Vector2 bedTile = new Vector2(bedPosition.x, bedPosition.z);
        if( (!ai.isAtTile(bedTile)) )
        {
            ai.walkToPosition(bedPosition);
            return false;
        }

        // We are at beds position
        return true;
    }

}
