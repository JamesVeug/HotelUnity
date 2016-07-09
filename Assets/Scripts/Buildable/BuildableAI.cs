using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class BuildableAI : Buildable, ICloneable, IEquatable<BuildableAI>
{
    private bool initialized = false;

    protected GameData data;
    protected SelectionTile selectionScript; 

    public abstract DRectangle Create(int x, int y);

    private void initialize()
    {

        data = GameObject.FindObjectOfType<GameData>();
        selectionScript = GameObject.FindObjectOfType<SelectionTile>();
        initialized = true;
    }

    public override void moveMouse(Vector3 movePosition)
    {
        if (!initialized) { initialize(); }
    }

    public override void pressMouse(Vector3 pressPosition)
    {
        if (!initialized) { initialize(); }
    }

    public override void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition)
    {
        if (!initialized) { initialize(); }

        Vector3 position = new Vector3(Mathf.Floor(releasePosition.x), 0, Mathf.Floor(releasePosition.z));
        data.gameLogic.addHouseKeeper(position);
    }

    public override void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        if (!initialized) { initialize(); }
    }

    public override void applyStage()
    {
        if (!initialized) { initialize(); }
    }

    public override bool hasNextStage()
    {
        return false;
    }

    public override int getStage()
    {
        return 0;
    }

    public override string getProperty()
    {
        return "BuildableAI";
    }

    public override void switchValue()
    {
        
    }

    public object Clone()
    {
        BuildableAI item = (BuildableAI)this.MemberwiseClone();
        return item;
    }

    public bool Equals(BuildableAI other)
    {
        return position.Equals(other.position);
    }

    public override bool canBeBuilt()
    {
        return selectionScript.isValid();
    }
}
