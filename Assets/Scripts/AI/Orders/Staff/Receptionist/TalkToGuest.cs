using UnityEngine;
using System.Collections;

public class TalkToGuest : Order {
    
	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        ReceptionistAI staff = (ReceptionistAI)ai;

        // Assign a post if we don't have one
        if( staff.post == null)
        {
            BReceptionRoom room = nav.getNearestEmptyFrontDeskRoom(ai);
            BuildableReception frontDesk = room.assignReceptionist(staff);
            staff.post = frontDesk;
        }

        // Walk to our post
        if ((ai.transform.position - staff.post.position).magnitude > 0.1)
        {
            Vector2 pos = staff.post.getStaffPosition();
            ai.walkToPosition(new Vector3(pos.x,0,pos.y));
            return RETURN_TYPE.PROBLEM;
        }
        


        // Should be in reception now
        return RETURN_TYPE.COMPLETED;
    }
}
