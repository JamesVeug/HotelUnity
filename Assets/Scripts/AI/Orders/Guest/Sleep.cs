﻿using UnityEngine;
using System.Collections;
using System;

public class Sleep : Order
{
	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        if (ai.property_sleep < 1)
        {
            // Go to sleep
            ai.property_sleep = Mathf.Min(1,ai.property_sleep+Time.deltaTime);
            ai.State = ai.STATE_SLEEPING;
			return RETURN_TYPE.PROBLEM;
        }

        // Wake up
        ai.State = ai.STATE_IDLE;
        return RETURN_TYPE.COMPLETED;
    }
}
