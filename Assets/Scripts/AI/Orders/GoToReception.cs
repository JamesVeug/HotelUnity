using UnityEngine;
using System.Collections;

public class GoToReception : Order {


	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {

        if (!(ai.getCurrentRoom() is BReceptionRoom))
        {

            BReceptionRoom reception = nav.getNearestReceptionRoom(ai);
            if (reception == null)
            {
                //Debug.Log("No Reception!");
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
