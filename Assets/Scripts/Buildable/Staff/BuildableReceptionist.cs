using UnityEngine;
using System.Collections;
using System;

public class BuildableReceptionist : BuildableStaff
{
    public override void createAI(Vector3 position)
    {
        data.gameLogic.addReceptionist(position);
    }
}
