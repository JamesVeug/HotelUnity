﻿using UnityEngine;
using System.Collections;
using System;

public class BuildableTile : Buildable
{
    public static string PROPERTY = "BuildableTile_Property";
    public static int STAGE = 0;

    public Vector2 changedTile = Vector2.zero;
    public int changedValue = 0;
    private int maxValue;
    
    public override void applyStage()
    {
        throw new NotImplementedException();
    }

    public override void cancelStage()
    {
        throw new NotImplementedException();
    }

    public override bool canBeBuilt()
    {
        return true;
    }

    public override void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        //throw new NotImplementedException();
        changedTile = new Vector2(dragPosition.x, dragPosition.z);
    }

    public override string getProperty()
    {
        return PROPERTY;
    }

    public override int getStage()
    {
        return STAGE;
    }

    public override bool hasNextStage()
    {
        return true;
    }

    public override void moveMouse(Vector3 movePosition)
    {
        //throw new NotImplementedException();
    }

    public override void pressMouse(Vector3 pressPosition, MouseButton mouseButton)
    {
        //throw new NotImplementedException();
        changedTile = new Vector2(pressPosition.x, pressPosition.z);
    }

    public override void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition, MouseButton mouseButton)
    {
        //Debug.Log("Release");

        //throw new NotImplementedException();
    }

    public override void switchValue()
    {
        Debug.Log("SWITCHING");
        changedValue++;
        if (changedValue >= maxValue)
        {
            changedValue = 0;
        }
    }

    public void setMaxValue(int v)
    {
        maxValue = v;
    }
}
