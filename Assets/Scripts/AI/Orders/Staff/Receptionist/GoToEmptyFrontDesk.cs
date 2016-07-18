using UnityEngine;
using System.Collections;

public class GoToEmptyFrontDesk : Order {

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        if (!(ai.getCurrentRoom() is BReceptionRoom))
        {

            BReceptionRoom reception = nav.getNearestEmptyFrontDeskRoom(ai);
            if (reception == null)
            {
                return RETURN_TYPE.FAILED;
            }

            Vector3 pos = nav.getClosestDoorPosition(ai, reception);
            ai.walkToPosition(pos);
            return RETURN_TYPE.PROBLEM;
        }

        // Should be in reception now
        return RETURN_TYPE.COMPLETED;
    }
}
