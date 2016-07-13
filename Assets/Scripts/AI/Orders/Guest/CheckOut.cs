using UnityEngine;
using System.Collections;

public class CheckOut : Order
{

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        // Walk to reception
        if (!(ai.getCurrentRoom() is BReceptionRoom))
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToReception>());
			return RETURN_TYPE.PROBLEM;
        }

        // Walk to front desk
        BReceptionRoom reception = (BReceptionRoom)ai.getCurrentRoom();
        BuildableReception frontDesk = reception.getFrontDesk(0);
        Vector2 position = frontDesk.getGuestPosition();
        if( !ai.isAtTile(position) )
        {
            //Debug.Log("Walk to front desk");
            Vector3 worldPosition = new Vector3(position.x, 0, position.y);
            ai.walkToPosition(worldPosition);
			return RETURN_TYPE.PROBLEM;
        }

        //// Clean the bed
        //BuildableBed bed = ai.getOwnedBed();
        //bed.setRoomToClean();

        // Check ai out of room
        ai.checkOut();
        
		return RETURN_TYPE.COMPLETED;
    }
}
