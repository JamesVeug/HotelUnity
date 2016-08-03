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
    public AIBase selectedAI;

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
        if( selectedItem != null)
        {
            selectedItem.cancelStage();
            selectedItem = null;
        }
        else if (selectedRoom != null)
        {
            selectedRoom.cancelStage();
        }
    }

    public override bool canBeBuilt()
    {
        return true;
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

    public override void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        if (selectedItem != null)
        {
            selectedItem.dragMouse(pressedPosition, dragPosition);
        }
    }

    public override void moveMouse(Vector3 movePosition)
    {
        
    }

    public override void pressMouse(Vector3 pressPosition, MouseButton mouseButton)
    {
        if( selectedItem != null)
        {
            selectedItem.pressMouse(pressPosition, mouseButton);
        }
    }

    public override void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition, MouseButton mouseButton)
    {

        if( selectedAI == null && selectedRoom == null && selectedItem == null)
        {
            AIBase ai = GetAIAtPosition(releasePosition);
            if( ai != null)
            {
                selectedAI = ai;
                return;
            }
        }
        else if( selectedAI != null )
        {
            // Did we click away from the ai?
            AIBase ai = GetAIAtPosition(releasePosition);
            if (ai != selectedAI)
            {
                // Deselect
                selectedAI = ai;
            }
            return;
        }

        // Look for room
        if (selectedRoom == null) { 
            BuildableRoom room = data.dTileMap.getRoom((int)pressedPosition.x, (int)pressedPosition.z);
            selectedRoom = room;
            Debug.Log("Room -> " + room);
        }

        // Have we selected a item
        if (selectedItem != null)
        {
            // Did we press the item?
            if (selectedItem.contains(releasePosition))
            {
                // Run press on that item
                selectedItem.releaseMouse(pressedPosition, releasePosition, mouseButton);
                return;
            }
            else
            {
                // Deselect Item
                selectedItem = null;
            }

        }

        // Have we already selected a room?
        if (selectedRoom != null)
        {

            // Did we click inside room
            if (selectedRoom.contains(pressedPosition))
            {
                // Look for item in the room
                BuildableItem item = data.dTileMap.getItem((int)pressedPosition.x, (int)pressedPosition.z, selectedRoom);
                if (item != null)
                {
                    Debug.Log("Selected Item");
                    selectedItem = item;
                    selectedItem.resetBuildTools();
                    return;
                }
            }
            else
            {
                // Deselect room
                selectedRoom = null;
            }
        }


    }

    private AIBase GetAIAtPosition(Vector3 position)
    {
        LayerMask layerMask = 1 << LayerMask.NameToLayer("AI");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Transform parent = hit.transform.root;
            AIBase ai = parent.GetComponent<AIBase>();
            if( ai != null)
            {
                return ai;
            }
            Debug.Log("Raycast Hit : " + parent.name);
        }


        return null;
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
