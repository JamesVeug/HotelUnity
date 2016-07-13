using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class BuildableItem : Buildable, ICloneable, IEquatable<BuildableItem>
{
    public Quaternion rotation = Quaternion.identity;
    protected Vector2 origin;
    protected List<Vector2> tiles = new List<Vector2>(); // All tiles related to the Object
    protected List<Vector2> itemTiles = new List<Vector2>(); // Tiles for the floor
    public GameObject gameObject;

    public abstract DRectangle Create(int x, int y);

    public List<Vector2> getTiles()
    {
        List<Vector2> t = new List<Vector2>();
        foreach(Vector2 tile in tiles)
        {
            t.Add(new Vector2(tile.x + left, tile.y + top));
        }
        return t;
    }

    public List<Vector2> getItemTiles()
    {
        List<Vector2> t = new List<Vector2>();
        foreach (Vector2 tile in itemTiles)
        {
            t.Add(new Vector2(tile.x + left, tile.y + top));
        }
        return t;
    }

    public Vector2 getOrigin()
    {
        return new Vector2(left + origin.x, top + origin.y);
    }

    public override void moveMouse(Vector3 movePosition)
    {
        throw new NotImplementedException();
    }

    public override void pressMouse(Vector3 pressPosition, MouseButton mouseButton)
    {
        throw new NotImplementedException();
    }

    public override void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition, MouseButton mouseButton)
    {
        throw new NotImplementedException();
    }

    public override void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        throw new NotImplementedException();
    }

    public override void applyStage()
    {
        throw new NotImplementedException();
    }
    
    public override bool hasNextStage()
    {
        throw new NotImplementedException();
    }

    public override int getStage()
    {
        throw new NotImplementedException();
    }

    public override string getProperty()
    {
        throw new NotImplementedException();
    }

    public override void switchValue()
    {
        throw new NotImplementedException();
    }

    public override void cancelStage()
    {
        throw new NotImplementedException();
    }

    public object Clone()
    {
        BuildableItem item = (BuildableItem)this.MemberwiseClone();
        item.rotation = Quaternion.identity*rotation;
        return item;
    }

    public bool Equals(BuildableItem other)
    {
        return position.Equals(other.position) && rotation.Equals(other.rotation);
    }

    public override bool canBeBuilt()
    {
        throw new NotImplementedException();
    }
}
