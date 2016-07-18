using UnityEngine;
using System.Collections;

public class GoToHouseKeeping : Order {

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        if (!(ai.getCurrentRoom() is BHouseKeepingRoom))
        {

            BHouseKeepingRoom hk = nav.getNearestHouseKeepingRoom(ai);
            if (hk == null)
            {
				return RETURN_TYPE.PROBLEM;
            }

            Vector3 pos = nav.getClosestDoorPosition(ai, hk);
            ai.walkToPosition(pos);

			return RETURN_TYPE.PROBLEM;
        }

        // Should be in House Keeping now
		return RETURN_TYPE.COMPLETED;
    }
}
