using UnityEngine;
using System.Collections;

public class CheckForDirtyRooms : Order {

    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        // Walk to House Keeping
        if (!(ai.getCurrentRoom() is BHouseKeepingRoom))
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToHouseKeeping>());
            return false;
        }
        
        // At House Keeping
        return true;
    }
}
