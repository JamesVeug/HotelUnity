using UnityEngine;
using System.Collections;

public class CheckForDirtyRooms : Order {

    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        // Walk to reception
        if (!(ai.getCurrentRoom() is BHouseKeepingRoom))
        {
            ai.addOrder(ScriptableObject.CreateInstance<GoToHouseKeeping>());
            return false;
        }

        // Walk to front desk
        /*BReceptionRoom reception = (BReceptionRoom)ai.getCurrentRoom();
        BuildableReception frontDesk = reception.getFrontDesk(0);
        Vector2 position = frontDesk.getGuestPosition();
        if (!ai.isAtTile(position))
        {
            //Debug.Log("Walk to front desk");
            Vector3 worldPosition = new Vector3(position.x, 0, position.y);
            ai.walkToPosition(worldPosition);
            return false;
        }

        // Need to allocate bed to player
        bool boughtRoom = ai.buyRoom();*/

        return true;
    }
}
