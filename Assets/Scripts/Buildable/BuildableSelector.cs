using UnityEngine;
using System.Collections;
using System;

public class BuildableSelector : Buildable
{
    public static string PROPERTY = "BuildableSelector_Selecting";
    public static int STAGE = 0;

    public Vector2 changedTile = Vector2.zero;
    public int changedValue = 0;
    private int maxValue;

    private GameData data;
    public BuildableRoom selectedRoom;
    public BuildableItem selectedItem;

    public void OnEnable()
    {
        data = FindObjectOfType<GameData>();
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
        //throw new NotImplementedException();
        //changedTile = new Vector2(dragPosition.x, dragPosition.z);
    }

    public override string getProperty()
    {
        return PROPERTY;
    }

    public override int getStage()
    {
        if (selectedItem != null)
        {
            return selectedItem.getStage();
        }
        else if (selectedRoom != null)
        {
            return selectedRoom.getStage();
        }
        return STAGE;
    }

    public override bool hasNextStage()
    {
        if( selectedItem != null)
        {
            return selectedItem.hasNextStage();
        }
        else if( selectedRoom != null ){
            return selectedRoom.hasNextStage();
        }
        return true;
    }

    public override void moveMouse(Vector3 movePosition)
    {
        
    }

    public override void pressMouse(Vector3 pressPosition, MouseButton mouseButton)
    {
        // Have we selected a item
        if(selectedItem != null)
        {
            // Did we press the item?
            if(selectedItem.contains(pressPosition))
            {
                // Run press on that item
                selectedItem.pressMouse(pressPosition, mouseButton);
                return;
            }

        }

        // Have we already selected a room?
        if( selectedRoom != null ){

            // Did we click inside room
            if( selectedRoom.contains(pressPosition))
            {
                // Look for item in the room
                BuildableItem item = data.dTileMap.getItem((int)pressPosition.x, (int)pressPosition.z, selectedRoom);
                if( item != null ){
                    Debug.Log("Selected Item");
                    selectedItem = item;
                    return;
                }
            }

            // Run press mouse inside the room
            // Else
            // Deselect room

        }


        // Look for room
        BuildableRoom room = data.dTileMap.getRoom((int)pressPosition.x, (int)pressPosition.z);
        selectedRoom = room;
        Debug.Log("Room -> " + room);

        // Got room
        // Look for item in room
        

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
