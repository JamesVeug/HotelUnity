using UnityEngine;
using System.Collections;
using System;

public class BuildableMaid : BuildableStaff
{
    public override void createAI(Vector3 position)
    {
        data.gameLogic.addHouseKeeper(position);
    }
}
