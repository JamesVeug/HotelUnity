using UnityEngine;
using System.Collections;
using System;

public class LookAtTile : Order {

    private Vector3 tile;

    public static LookAtTile create(Vector3 tile)
    {
        LookAtTile o = CreateInstance<LookAtTile>();
        return o.setTile(tile);
    }

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        if( !isLookingAtTile(ai.transform,tile))
        {
            Quaternion currentRotation = ai.transform.rotation;
            float degree = Quaternion.LookRotation((tile - ai.transform.position).normalized).eulerAngles.y;
            ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, Quaternion.Euler(currentRotation.x, degree, currentRotation.z), Time.deltaTime * 20f);
            return RETURN_TYPE.PROBLEM;
        }

        return RETURN_TYPE.COMPLETED;
    }

    public static bool isLookingAtTile(Transform transform, Vector3 target)
    {
        Quaternion currentRotation = transform.rotation;
        float degree = Quaternion.LookRotation((target - transform.position).normalized).eulerAngles.y;
        if (Mathf.Abs(currentRotation.eulerAngles.y - degree) > 5)
        {
            return false;
        }

        // Not facing the position
        return true;
    }

    public LookAtTile setTile(Vector3 tile)
    {
        this.tile = tile;
        return this;
    }
}
