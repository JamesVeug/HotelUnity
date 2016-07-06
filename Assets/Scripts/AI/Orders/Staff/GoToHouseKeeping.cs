using UnityEngine;
using System.Collections;

public class GoToHouseKeeping : Order {

    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        if (!(ai.getCurrentRoom() is BHouseKeepingRoom))
        {

            BHouseKeepingRoom hk = nav.getNearestHouseKeepingRoom(ai);
            if (hk == null)
            {
                Debug.Log("No House Keeping!");
                return false;
            }

            Vector3 pos = nav.getClosestDoorPosition(ai, hk);
            ai.walkToPosition(pos);
            return false;
        }

        // Should be in reception now
        return true;
    }
}
