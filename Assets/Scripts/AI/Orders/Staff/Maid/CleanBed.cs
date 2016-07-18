using UnityEngine;
using System.Collections;

public class CleanBed : Order {

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        MaidAI staff = (MaidAI)ai;


        // Walk to bed
        Vector2 bedTile = staff.dirtyBed.getBedPosition(0);
        Vector3 bedPosition = new Vector3(bedTile.x, 0, bedTile.y);
        if ((!ai.isByTile(bedTile)))
        {
            ai.walkToItem(bedPosition);
            return RETURN_TYPE.PROBLEM;
        }

        // Make sure it's still dirty
        if( !staff.dirtyBed.isDirty )
        {
            return RETURN_TYPE.COMPLETED;
        }

        // Face Bed
        if( !LookAtTile.isLookingAtTile(ai.transform,bedPosition))
        {
            ai.addOrder(LookAtTile.create(bedPosition));
            return RETURN_TYPE.PROBLEM;
        }

        // Clean it
        staff.dirtyBed.setRoomToClean();

        return RETURN_TYPE.COMPLETED;
    }
}
