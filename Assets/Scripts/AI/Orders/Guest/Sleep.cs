using UnityEngine;
using System.Collections;
using System;

public class Sleep : Order
{
    public override bool executeOrder(AIBase ai, Navigation nav)
    {
        if (ai.property_sleep < 1)
        {
            // Go to sleep
            ai.property_sleep = Mathf.Min(1,ai.property_sleep+Time.deltaTime);
            return false;
        }

        // Wake up
        return true;
    }
}
