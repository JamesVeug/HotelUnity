using UnityEngine;
using System.Collections;

public class CheckForDirtyRooms : Order {

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        // Walk to House Keeping
        if (!(ai.getCurrentRoom() is BHouseKeepingRoom))
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToHouseKeeping>());
            return RETURN_TYPE.PROBLEM;
        }

        // Need to allocate room to the staff
        if( ai is MaidAI)
        {
            MaidAI staff = (MaidAI)ai;
            return toReturnType(staff.assignDirtyRoom());
        }

        // We aren't staff
        return RETURN_TYPE.FAILED;
    }
}
