using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuildableItem : DRectangle, Buildable, ICloneable, IEquatable<BuildableItem>
{
    public Quaternion rotation = Quaternion.identity;
    protected List<Vector2> tiles = new List<Vector2>();
    public BuildableItem(int x, int y)
    {
        this.left = x;
        this.top = y;
        this.width = 1;
        this.height = 1;
        //rotation.eulerAngles += Quaternion.Euler(0, 90, 0).eulerAngles;
    }

    public List<Vector2> getTiles()
    {
        List<Vector2> t = new List<Vector2>();
        foreach(Vector2 tile in tiles)
        {
            t.Add(new Vector2(tile.x + left, tile.y + top));
        }
        return t;
    }

    public void moveMouse(Vector3 movePosition)
    {
        throw new NotImplementedException();
    }

    public void pressMouse(Vector3 pressPosition)
    {
        throw new NotImplementedException();
    }

    public void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition)
    {
        throw new NotImplementedException();
    }

    public void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        throw new NotImplementedException();
    }

    public void applyStage()
    {
        throw new NotImplementedException();
    }
    

    public bool hasNextStage()
    {
        throw new NotImplementedException();
    }

    public int getStage()
    {
        throw new NotImplementedException();
    }

    public string getProperty()
    {
        throw new NotImplementedException();
    }

    public void switchValue()
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
}
