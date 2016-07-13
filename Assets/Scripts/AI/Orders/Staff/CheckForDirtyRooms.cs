using UnityEngine;
using System.Collections;

public class CheckForDirtyRooms : Order {

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        // Walk to House Keeping
        if (!(ai.getCurrentRoom() is BHouseKeepingRoom))
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToHouseKeeping>());
            if(ai.getCurrentRoom() != null )
                Debug.Log("In " + ai.getCurrentRoom().GetType().Name);
            else
            {
                Debug.Log("Null room");
            }
            return RETURN_TYPE.PROBLEM;
        }

        // Need to allocate room to the staff
        if( ai is StaffAI)
        {
            StaffAI staff = (StaffAI)ai;
            return toReturnType(staff.assignDirtyRoom());
        }

        // We aren't staff
        return RETURN_TYPE.FAILED;
    }
}
