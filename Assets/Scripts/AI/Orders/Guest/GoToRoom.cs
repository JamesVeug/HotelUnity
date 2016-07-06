using UnityEngine;
using System.Collections;

public class GoToRoom : Order {


    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        if (ai.getCurrentRoom() != ai.getOwnedRoom() )
        {

            Vector3 pos = nav.getClosestDoorPosition(ai, ai.getOwnedRoom());
            ai.walkToPosition(pos);
            return false;
        }

        // Should be in reception now
        return true;
    }
}
