﻿using UnityEngine;
using System.Collections;

public class CleanBed : Order {

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        StaffAI staff = (StaffAI)ai;


        // Walk to bed
        Vector2 bedTile = staff.dirtyBed.getBedPosition(0);
        Vector3 bedPosition = new Vector3(bedTile.x, 0, bedTile.y);
        if ((!ai.isByTile(bedTile)))
        {
            ai.walkToItem(bedPosition);
            return RETURN_TYPE.PROBLEM;
        }

        // Face Bed
        Quaternion currentRotation = staff.transform.rotation;
        float degree = Quaternion.LookRotation((bedPosition- staff.transform.position).normalized).eulerAngles.y;
        if (currentRotation.eulerAngles.y != degree)
        {
            staff.transform.rotation = Quaternion.Euler(currentRotation.x,degree, currentRotation.z);

        }

        // Clean it
        staff.dirtyBed.setRoomToClean();

        return RETURN_TYPE.COMPLETED;
    }
}