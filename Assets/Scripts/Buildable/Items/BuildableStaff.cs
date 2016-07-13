using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuildableStaff : Buildable
{
    public static string PROPERTY = "BuildableTile_Property";
    public static int STAGE = 0;
    
    public int changedValue = 0;
    private int maxValue;
    private bool initialized = false;

    protected GameData data;
    protected SelectionTile selectionScript;

    private void initialize()
    {

        selectionScript = GameObject.FindObjectOfType<SelectionTile>();
        data = GameObject.FindObjectOfType<GameData>();
        initialized = true;
    }

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
		if (!initialized) initialize();
		selectionScript.rect.position = movePosition;// - cubeSize;
		selectionScript.rect.size = new Vector3(1,0,1);
    }

    public override void pressMouse(Vector3 pressPosition, MouseButton mouseButton)
    {
        if (!initialized) initialize();

		if (selectionScript.isValid ()) {
			data.gameLogic.addHouseKeeper (pressPosition);
		}
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
