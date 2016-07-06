using UnityEngine;
using System.Collections;

public class GoToReception : Order {


    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        if (!(ai.getCurrentRoom() is BReceptionRoom))
        {

            BReceptionRoom reception = nav.getNearestReceptionRoom(ai);
            if (reception == null)
            {
                //Debug.Log("No Reception!");
                return false;
            }

            Vector3 pos = nav.getClosestDoorPosition(ai, reception);
            ai.walkToPosition(pos);
            return false;
        }

        // Should be in reception now
        return true;
    }
}
