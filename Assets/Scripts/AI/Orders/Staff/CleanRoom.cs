using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CleanRoom : Order {

    private bool initialized = false;
    private List<int> dirtyBeds = new List<int>();

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {

        if (!(ai is StaffAI))
        {
            return RETURN_TYPE.FAILED;
        }

        StaffAI staff = (StaffAI)ai;
        

        // Make sure we have a room to clean
        if( staff.dirtyRoom == null)
        {
            ai.addOrder(ScriptableObject.CreateInstance<CheckForDirtyRooms>());
            return RETURN_TYPE.PROBLEM;
        }

        // Make sure we are in the room
        if( ai.getCurrentRoom() != staff.dirtyRoom)
        {
            Vector3 pos = nav.getClosestDoorPosition(ai, staff.dirtyRoom);
            ai.walkToPosition(pos);
            return RETURN_TYPE.PROBLEM;
        }

        // Record each of the beds so we know that we have to clean each of them at most once
        if( !initialized)
        {
            int beds = staff.dirtyRoom.bedCount();
            for(int i = 0; i < beds; i++)
            {
                BuildableBed bed = staff.dirtyRoom.getBed(i);
                if (bed.isDirty && !bed.isSoldToGuest())
                {
                    Debug.Log("Dirty Bed " + i);
                    dirtyBeds.Add(i);
                }
            }

            initialized = true;
        }

        // Clean bed
        if (dirtyBeds.Count > 0)
        {
            int index = getClosestBedIndex(staff);
            BuildableBed bed = staff.dirtyRoom.getBed(dirtyBeds[index]); // Disgusting code... blachhhhh
            if (!bed.isDirty )
            {
                // Bed already clean. Ignore it
                staff.dirtyBed = null;
                dirtyBeds.RemoveAt(index);
                return RETURN_TYPE.PROBLEM;
            }

            // Clean bed
            staff.dirtyBed = bed;
            ai.addOrder(ScriptableObject.CreateInstance<CleanBed>());
            return RETURN_TYPE.PROBLEM;
        }

        // Room is clean
        return RETURN_TYPE.COMPLETED;
    }

    private int getClosestBedIndex(StaffAI staff)
    {
        float distance = float.MaxValue;
        int index = 0;

        for (int i = 0; i < dirtyBeds.Count; i++)
        {
            BuildableBed bed = staff.dirtyRoom.getBed(dirtyBeds[i]);
            float d = (bed.position - staff.transform.position).magnitude;
            if( d < distance)
            {
                index = i;
                distance = d;
            }
        }

        return index;
    }
}
