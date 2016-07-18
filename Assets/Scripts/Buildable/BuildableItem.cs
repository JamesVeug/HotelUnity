using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class BuildableItem : Buildable, ICloneable, IEquatable<BuildableItem>
{
    private const string Property_Modify = "BuildtableItem_Modify";
    private const int Stage_Modify = 0;

    private int Stage = 0;
    private string Property = Property_Modify;

    public Quaternion rotation = Quaternion.identity;
    protected Vector2 origin;
    protected List<Vector2> tiles = new List<Vector2>(); // All tiles related to the Object
    protected List<Vector2> itemTiles = new List<Vector2>(); // Tiles for the floor
    public GameObject gameObject;

    private GameData data;

    public abstract DRectangle Create(int x, int y);

    public void resetBuildTools()
    {
        Stage = Stage_Modify;
        Property = Property_Modify;
        if( data == null)
        {
            data = FindObjectOfType<GameData>();
        }
    }

    public List<Vector2> getTiles()
    {
        List<Vector2> t = new List<Vector2>();
        foreach(Vector2 tile in tiles)
        {
            float x = tile.x + left;
            float y = tile.y + top;
            Vector2 position = new Vector2(x, y);
            Vector2 rotatedPosition = Vector2Extension.Rotate(position,getOrigin(),rotation.eulerAngles.y);
            t.Add(rotatedPosition);
        }
        return t;
    }

    public List<Vector2> getItemTiles()
    {
        List<Vector2> t = new List<Vector2>();
        foreach (Vector2 tile in itemTiles)
        {
            Vector2 position = new Vector2(tile.x + left, tile.y + top);
            Vector2 rotatedPosition = Vector2Extension.Rotate(position, getOrigin(), rotation.eulerAngles.y);
            t.Add(rotatedPosition);
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
        //throw new NotImplementedException();
    }

    public override void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition, MouseButton mouseButton)
    {
        if (pressedPosition.Equals(releasePosition))
        {
            data.dTileMap.RemoveItem(this);
            rotation = rotation * Quaternion.Euler(0, 90, 0);

            // Add more changes
            data.dTileMap.AddItem(this);
        }
    }

    public override void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        //throw new NotImplementedException();
        if( data.dTileMap.CanPlaceItem(this,new Vector2(dragPosition.x, dragPosition.z)) )
        {

            data.dTileMap.RemoveItem(this);
            this.left = (int)dragPosition.x;
            this.top = (int)dragPosition.z;
            data.dTileMap.AddItem(this);
        }
    }

    public override void applyStage()
    {
        Stage++;
    }
    
    public override bool hasNextStage()
    {
        return Stage == 0;
    }

    public override int getStage()
    {
        return Stage;
    }

    public override string getProperty()
    {
        return Property;
    }

    public override void switchValue()
    {
        throw new NotImplementedException();
    }

    public override void cancelStage()
    {

        data.dTileMap.RemoveItem(this);
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
