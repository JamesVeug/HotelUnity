using UnityEngine;
using System.Collections;

public class GoToRoom : Order {


	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        if (ai.getCurrentRoom() != ai.getOwnedRoom() )
        {

            Vector3 pos = nav.getClosestDoorPosition(ai, ai.getOwnedRoom());
            ai.walkToPosition(pos);
			return RETURN_TYPE.PROBLEM;
        }

        // Should be in reception now
		return RETURN_TYPE.COMPLETED;
    }
}
